using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Script.DossierPoint;
using Script.EntityPlayer;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Script.InterfaceInGame;
using Script.TeteChercheuse;
using Script.Tools;
using Random = System.Random;

namespace Script.Manager
{
    public class MasterManager : MonoBehaviour
    {
        public static MasterManager Instance;
        
        [Header("Prefab")]
        [SerializeField] private BodyRectilgne originalBodyRectilgne; // prefab des bodyRectiligne
        [SerializeField] private BodyGaz originalBodyGaz; // prefab des bodyGaz
        [SerializeField] private RayGaz originalRayGaz; // prefab des RayGaz
        public GameObject marqueur;
        public GameObject PointPath;

        [Header("Dossier")]
        [SerializeField] private Transform dossierBodyChercheur; // ranger les 'BodyChercheur'
        [SerializeField] private Transform dossierBalleFusil; // ranger les 'BalleFusil'
        [SerializeField] private Transform dossierRayGaz; // ranger les marqueurs des 'RayGaz'
        [SerializeField] private Transform dossierOtherBot; // le dossier où les bots que ton ordinateur ne contrôle pas seront rangés
        
        [Header("Bot")]
        [SerializeField] private CapsuleCollider capsuleBot;
        
        [Header("Scene")]
        [SerializeField] private TypeScene scene;
        
        // les contours de la scène (notamment utilisé par le gaz)
        private (float minZ, float minX, float maxZ, float maxX) contour;

        // nombre de participant (sera utilisé pour déterminer le moment lorsque tous les joueurs auront instancié leur playerController)
        private int nParticipant; // participant regroupe les joueurs ainsi que les spectateurs

        // Accéder aux différents joueurs, chaque joueur sera donc stocké deux fois, sauf s'il est mort, il sera juste un spectateur
        private List<PlayerClass> players;
        private List<Chasseur> chasseurs;
        private List<Chassé> chassés;
        private List<Spectateur> spectateurs;

        // attribut relatif à ton avatar
        private PlayerClass ownPlayer;
        
        // cette liste va servir à donner les noms à chaque bot
        private int[] nBotParJoueur;

        // pour savoir sur quel scène nous sommes
        public enum TypeScene
        {
            Game,
            Labyrinthe,
            Maintenance
        }

        private ManagerGame typeScene;

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
        public Transform GetDossierRayGaz() => dossierRayGaz;
        public Transform GetDossierOtherBot() => dossierOtherBot;
        public (float, float, float, float) GetContour() => contour;
        public HumanCapsule GetHumanCapsule() => new HumanCapsule(capsuleBot);
        public TypeScene GetTypeScene() => scene;
        public ManagerGame GetManagerGame() => typeScene;
        public string GetNameBot(Player player)
        {
            int i;
            for (i = 0; i < nParticipant && !PhotonNetwork.PlayerList[i].Equals(player); i++)
            {} // cherche l'index du joueur
            
            nBotParJoueur[i] += 1;
            return player.NickName + "Bot" + nBotParJoueur[i];
        }

        public bool IsInMaintenance() => typeScene is InMaintenance;
        
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

            if (typeScene is InLabyrinthe)
                return;
            
            // ce if s'active lorsque tous les joueurs ont créé leur avatar et l'ont ajouté à la liste 'players'
            if (players.Count == nParticipant)
            {
                InterfaceInGameManager.Instance.Set();
            }
        }
        public void AjoutChasseur(Chasseur chasseur)
        {
            chasseurs.Add(chasseur);
        }
        public void AjoutChassé(Chassé chassé)
        {
            chassés.Add(chassé);
        }

        // constructeur
        private void Awake()
        {
            // On peut faire ça puisqu'il y en aura qu'un seul
            Instance = this;
            
            // instancier le nombre de joueur
            nParticipant = PhotonNetwork.PlayerList.Length;
            
            if (CrossManager.IsMaintenance()) // maintenance des crossPoints
            {
                Debug.Log("Début Maintenance des CrossPoints");
                typeScene = new InMaintenance(nParticipant);
            }
            else if (scene == TypeScene.Game) // guess who
            {
                typeScene = new InGuessWho(nParticipant);
            }
            else // labyrinthe
            {
                typeScene = new InLabyrinthe(nParticipant);
            }

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
            // récupérer les contours de la map
            RecupContour();
            
            // Seul le masterClient décide le type de chaque joueur
            if (!PhotonNetwork.IsMasterClient)
                return;

            SendInfoPlayer();
        }

        private void RecupContour()
        {
            Point[] contourPoint = GetComponentsInChildren<Point>();
            int l = contourPoint.Length;
            Vector3[] list = new Vector3[l];
            
            for (int i = 0; i < l; i++)
            {
                list[i] = contourPoint[i].transform.position;
            }

            // min
            contour.minZ = ManList.GetMin(list, ManList.Coord.Z);
            contour.minX = ManList.GetMin(list, ManList.Coord.X);
            // max
            contour.maxZ = ManList.GetMax(list, ManList.Coord.Z);
            contour.maxX = ManList.GetMax(list, ManList.Coord.X);
        }

        private void SendInfoPlayer()
        {
            // les spots
            int[] indexSpotChasseur = SpawnManager.Instance.GetBeginPosition(TypePlayer.Chasseur);
            int[] indexSpotChassé = SpawnManager.Instance.GetBeginPosition(TypePlayer.Chassé);

            // les types en fonction du type de la partie
            TypePlayer[] types = typeScene.GetTypePlayer();
            string infoJoueur;

            for (int i = 0; i < nParticipant; i++)
            {
                if (types[i] != TypePlayer.None) // on va devoir envoyer quelque chose au PlayerManager
                {
                    if (types[i] == TypePlayer.Chasseur) // chasseur
                    {
                        int indexSpot = indexSpotChasseur[i];
                        infoJoueur = PlayerManager.EncodeFormatInfoJoueur(indexSpot, TypePlayer.Chasseur);
                    }
                    else if (types[i] == TypePlayer.Chassé) // chassé
                    {
                        int indexSpot = indexSpotChassé[i];
                        infoJoueur = PlayerManager.EncodeFormatInfoJoueur(indexSpot, TypePlayer.Chassé);
                    }
                    else if (types[i] == TypePlayer.Blocard) // blocard
                    {
                        int indexSpot = indexSpotChassé[i];
                        infoJoueur = PlayerManager.EncodeFormatInfoJoueur(indexSpot, TypePlayer.Blocard);
                    }
                    else
                    {
                        throw new Exception($"Pas encore géré le cas du {types[i]}");
                    }
                    
                    // envoi des infos au concerné(e)
                    Hashtable hash = new Hashtable();
                    hash.Add("InfoCréationJoueur", infoJoueur);
                    PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                }
            }
        }

        private void Update()
        {
            // S'il y a maintenance, il n'y a pas de joueur et pas leur gestion
            if (IsInMaintenance())
                return;
        }

        public void Die(Player player)
        {
            int i;
            for (i = 0; i < players.Count && !players[i].GetPlayer().Equals(player); i++) //cherche le joueur dans la liste des players
            {}

            if (i == players.Count)
            {
                throw new Exception("Un script tente de supprimer un joueur de la liste qui n'y est plus");
            }

            PlayerClass playerClass = players[i];
            players.RemoveAt(i); // remove de la liste players

            if (playerClass is Chassé)
            {
                // remove de la liste chassés (si c'était un chassé)
                chassés.Remove((Chassé) playerClass);
            }
            else if (playerClass is Chasseur)
            {
                // remove de la liste chasseurs (si c'était un chasseur)
                chasseurs.Remove((Chasseur) playerClass);
            }
            else
            {
                throw new Exception($"La mort du type de {playerClass} n'est pas encore implémenté");
            }

            PhotonView pv = playerClass.GetPv(); // on récupère le point de vue du mourant

            if (!pv.IsMine) // Seul le mourant envoie le hash et créé un spectateur
                return;
            
            // création du spectateur
            Spectateur spectateur = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "Spectateur"),
                Vector3.zero, Quaternion.identity, 0, new object[]{pv.ViewID}).GetComponent<Spectateur>();
            
            // ajout à la liste 'spectateurs'
            spectateurs.Add(spectateur);

            // envoie du hash pour que les autres le supprime de leurs listes
            Hashtable hash = new Hashtable();
            hash.Add("MortPlayer", player);
            player.SetCustomProperties(hash);
        }
    }
}