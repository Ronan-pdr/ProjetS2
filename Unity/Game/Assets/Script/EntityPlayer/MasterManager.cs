using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Script.DossierPoint;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Script.InterfaceInGame;
using Script.TeteChercheuse;
using Script.Tools;
using Random = System.Random;

namespace Script.EntityPlayer
{
    public class MasterManager : MonoBehaviour
    {
        //Cela permet d'y accéder facilement
        public static MasterManager Instance;
        
        // les prefabs
        [SerializeField] private BodyRectilgne originalBodyRectilgne; // prefab des bodyRectiligne
        
        [SerializeField] private BodyGaz originalBodyGaz; // prefab des bodyGaz
        [SerializeField] private RayGaz originalRayGaz; // prefab des RayGaz
        [SerializeField] private GameObject[] contour;
        public GameObject marqueur;
        public GameObject PointPath;

        [SerializeField] private CapsuleCollider capsuleBot;

        [SerializeField] private Transform dossierBodyChercheur; // ranger les 'BodyChercheur'
        [SerializeField] private Transform dossierBalleFusil; // ranger les 'BalleFusil'
        
        [SerializeField] private Transform dossierOtherBot; // le dossier où les bots que ton ordinateur ne contrôle pas seront rangés
        
        // nombre de participant (sera utilisé pour déterminer le moment lorsque tous les joueurs auront instancié leur playerController)
        private int nParticipant; // participant regroupe les joueurs ainsi que les spectateurs

        // Accéder aux différents joueurs, chaque joueur sera donc stocké deux fois, sauf s'il est mort, il sera juste un spectateur
        private List<PlayerClass> players;
        private List<Chasseur> chasseurs;
        private List<Chassé> chassés;
        private List<Spectateur> spectateurs;

        // attribut relatif à ton avatar
        private PlayerClass ownPlayer;
        
        // attribut que seul le masterClient utilisera
        // la string contenant les infos du joueur seront sous la forme :
        // indexCoordPoint(2 caractères) + type(1 caractère)
        private string[] listInfoCréationJoueur;
        
        // les booléens qui indiquent ce que le MasterClient a fait ou non dans le Update
        private bool EnvoieDuTypeJoueurs; // Est ce que les hash indiquant le type des joueurs ont été envoyés ?
        private bool SetInterfaceInGame; // Est que l'interface in game a déjà été set ?
        
        // cette liste va servir à donner les noms à chaque bot
        private int[] nBotParJoueur;
        
        // si maintenance != None alors y'a pas de jeu
        private TypeMaintenance Maintenance;

        public enum TypeMaintenance
        {
            None,
            CrossManager // la recherche des voisins de chaque SpawnPoint
        }
        
        // Enum pour la création des joueurs
        public enum TypePlayer
        {
            Chasseur = 0,
            Chassé = 1,
            Spectateur = 2
        }

        // Getter
        public int GetNbParticipant() => nParticipant; // les spectateurs sont compris
        public int GetNbPlayer() => players.Count;
        public int GetNbChasseur() => chasseurs.Count;
        public int GetNbChassé() => chassés.Count;
        public PlayerClass GetOwnPlayer() => ownPlayer;
        public List<PlayerClass> GetListPlayer() => players;
        public PlayerClass GetPlayer(int index) => players[index];
        public Chasseur GetChasseur(int index) => chasseurs[index];
        public Chassé GetChassé(int index) => chassés[index];
        public BodyRectilgne GetOriginalBodyRectilgne() => originalBodyRectilgne;
        public BodyGaz GetOriginalBodyGaz() => originalBodyGaz;
        public RayGaz GetOriginalRayGaz() => originalRayGaz;
        public Transform GetDossierBodyChercheur() => dossierBodyChercheur;
        public Transform GetDossierBalleFusil() => dossierBalleFusil;
        public Transform GetDossierOtherBot() => dossierOtherBot;
        public GameObject[] GetContour() => contour;
        public CapsuleCollider GetCapsuleBot() => capsuleBot;
        public string GetNameBot(Player player)
        {
            int i;
            for (i = 0; i < nParticipant && !PhotonNetwork.PlayerList[i].Equals(player); i++)
            {} // cherche l'index du joueur
            
            nBotParJoueur[i] += 1;
            return player.NickName + "Bot" + nBotParJoueur[i];
        }

        public bool IsInMaintenance() => Maintenance != TypeMaintenance.None;
        public bool IsInCrossPointMaintenance() => Maintenance == TypeMaintenance.CrossManager;
        
        //Setter
        public void SetOwnPlayer(PlayerClass value)
        {
            if (ownPlayer is null)
                ownPlayer = value;
            else
                throw new Exception("Un script a tenté de réinitialiser la variable ownPLayer");
        }
        public void AjoutPlayer(PlayerClass player)
        {
            players.Add(player);
        }
        public void AjoutChasseur(Chasseur chasseur)
        {
            chasseurs.Add(chasseur);
        }
        public void AjoutChassé(Chassé chassé)
        {
            chassés.Add(chassé);
        }

        private void Awake()
        {
            // On peut faire ça puisqu'il y en aura qu'un seul
            Instance = this;
            
            if (CrossManager.IsMaintenance()) // maintenance des cross point
            {
                Debug.Log("Début Maintenance des CrossPoints");
                Maintenance = TypeMaintenance.CrossManager;
            }
            else
            {
                Maintenance = TypeMaintenance.None;
            }
            
            // pas de gestion de joueurs s'il y a maitenance
            if (IsInMaintenance())
                return;
            
            // instancier le nombre de joueur
            nParticipant = PhotonNetwork.PlayerList.Length;
            
            // instancier les listes
            players = new List<PlayerClass>();
            chasseurs = new List<Chasseur>();
            chassés = new List<Chassé>();
            spectateurs = new List<Spectateur>();
            
            // cette liste va servir à donner les noms à chaque bot
            nBotParJoueur = new int[nParticipant];
        }

        public void Start()
        {
            // Seul le masterClient décide le type de chaque joueur, il l'envoie aux autres dans 'Update'
            // et s'il y a maintenance, alors il n'y a pas de joueur
            if (!PhotonNetwork.IsMasterClient || IsInMaintenance())
                return;
            
            listInfoCréationJoueur = new string[nParticipant]; // instancier la liste
            List<int> listChasseur = ManList.CreateListRange(SpawnManager.Instance.GetLengthSpawnPointChasseur());
            List<int> listChassé = ManList.CreateListRange(SpawnManager.Instance.GetLengthSpawnPointChassé());

            int indexSpot;
            Random random = new Random();
            for (int i = 0; i < nParticipant; i++)
            {
                if (i == 0) // pour l'instant, seul le premier de liste est un chasseur 
                {
                    indexSpot = listChasseur[random.Next(listChasseur.Count)];
                    listChasseur.Remove(indexSpot);
                    listInfoCréationJoueur[i] = ManString.Format(indexSpot.ToString(), 2) + (int)TypePlayer.Chasseur;
                }
                else
                {
                    indexSpot = listChassé[random.Next(listChassé.Count)];
                    listChassé.Remove(indexSpot);
                    listInfoCréationJoueur[i] = ManString.Format(indexSpot.ToString(), 2) + (int)TypePlayer.Chassé;
                }
            }
        }

        private void Update()
        {
            // S'il y a maintenance, il n'y a pas de joueur et pas leur gestion
            if (IsInMaintenance())
                return;
            
            // J'ai implémenter des alertes qui indique les erreurs possible qui n'engendre pas forcément d'erreur de script
            if (players.Count > nParticipant)
            {
                Debug.Log("ALERTE (Update du MasterManager) :");
                Debug.Log($"Il y a {players.Count} joueurs pour {nParticipant} participants");
                Debug.Log("Vous préviendrez Sacha");
            }

            // ce if sert à indiqué à tous les participants leur type de joueur (chasseur ou chassé)
            // ce if s'active losque tout le monde est déplacé son 'PlayerManager' dans le 'MasterManager' et losqu'il l'a pas déjà fait
            // seul le masterClient l'active
            if (!EnvoieDuTypeJoueurs && PhotonNetwork.IsMasterClient && GetComponentsInChildren<PlayerManager>().Length == nParticipant)
            {
                for (int i = 0; i < nParticipant; i++) // envoie le type à tous les joueurs grâce à un hash
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("InfoCréationJoueur", listInfoCréationJoueur[i]);
                    PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                }

                EnvoieDuTypeJoueurs = true;
            }

            // ce if s'active lorsque tous les joueurs ont créé leur avatar et l'ont ajouté à la liste 'players'
            if (players.Count == nParticipant && !SetInterfaceInGame)
            {
                InterfaceInGameManager.Instance.Set();

                SetInterfaceInGame = true;
            }
        }

        public void Die(Player player)
        {
            int i;
            for (i = 0; i < players.Count && !players[i].GetPlayer().Equals(player); i++) //cherche le joueur dans la liste des players
            {}

            PlayerClass playerClass = players[i]; // S'il y a un problème d'index à cette ligne c'est que tu tentes de supprimer un joueur de la liste qui n'y est plus
            
            players.RemoveAt(i); // remove de la liste players

            if (playerClass is Chassé)
            {
                chassés.Remove((Chassé) playerClass); // remove de la liste chassés (si c'était un chassé)
            }
            else
            {
                chasseurs.Remove((Chasseur) playerClass); // remove de la liste chasseurs (si c'était un chasseur)
            }

            PhotonView pv = playerClass.GetPv(); // on récupère le point de vue du mourant

            if (!pv.IsMine) // Seul le mourant envoie le hash et créé un spectateur
                return;
            
            Spectateur spectateur = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "Spectateur"),
                Vector3.zero, Quaternion.identity, 0, new object[]{pv.ViewID}).GetComponent<Spectateur>(); // création du spectateur
            
            spectateurs.Add(spectateur); // ajout à la liste 'spectateurs'
            
            Hashtable hash = new Hashtable();
            hash.Add("MortPlayer", player);
            player.SetCustomProperties(hash); // envoie du hash pour que les autres le supprime de leurs listes
        }
    }
}