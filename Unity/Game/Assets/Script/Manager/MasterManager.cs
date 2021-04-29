using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using Script.Bot;
using Script.DossierPoint;
using Script.EntityPlayer;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Script.InterfaceInGame;
using Script.Labyrinthe;
using Script.Menu;
using Script.TeteChercheuse;
using Script.Tools;
using Random = System.Random;

namespace Script.Manager
{
    public class MasterManager : MonoBehaviour
    {
        // ------------ SerializedField ------------
        
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

        [Header("InterfaceInGame")]
        [SerializeField] private GameObject visé;
        
        // ------------ Attributs ------------
        
        public static MasterManager Instance;
        
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

        // pour savoir sur quel scène nous sommes
        public enum TypeScene
        {
            Game,
            Labyrinthe,
            Maintenance
        }

        private ManagerGame typeScene;

        private bool _endedGame;

        // ------------ Getters ------------
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
        public bool IsGameEnded() => _endedGame;

        public bool IsInMaintenance() => typeScene is InMaintenance;
        
        public bool IsMasterOfTheMaster(string n) => n.Contains("Peepoodoo");

        // ------------ Setters ------------
        public void SetVisée(bool value)
        {
            visé.SetActive(value);
        }
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
                TabMenu.Instance.Set();
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

        // ------------ Constructeurs ------------
        private void Awake()
        {
            // On peut faire ça puisqu'il y en aura qu'un seul
            Instance = this;
            
            // instancier le nombre de joueur
            nParticipant = PhotonNetwork.PlayerList.Length;

            // instancier les listes
            players = new List<PlayerClass>();
            chasseurs = new List<Chasseur>();
            chassés = new List<Chassé>();
            spectateurs = new List<Spectateur>();
        }

        public void Start()
        {
            // determiner le typeScene
            if (CrossManager.Instance.IsMaintenance) // maintenance des crossPoints
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
            
            // récupérer les contours de la map
            RecupContour();
            
            if (!PhotonNetwork.IsMasterClient)
                return;

            SendInfoPlayer();
            SendInfoBot();
        }

        // ------------ Méthodes ------------
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
            // les spawns
            int[] indexSpawnChasseur = SpawnManager.Instance.GetSpawnPlayer(TypePlayer.Chasseur);
            int iChasseur = 0;
            int[] indexSpawnChassé = SpawnManager.Instance.GetSpawnPlayer(TypePlayer.Chassé);
            int iChassé = 0;

            // les types en fonction du type de la partie
            TypePlayer[] types = typeScene.GetTypePlayer();
            int indexSpawn;

            for (int i = 0; i < nParticipant; i++)
            {
                if (types[i] != TypePlayer.None) // on va devoir envoyer quelque chose au PlayerManager
                {
                    if (types[i] == TypePlayer.Chasseur) // chasseur
                    {
                        indexSpawn = indexSpawnChasseur[iChasseur];
                        iChasseur++;
                    }
                    else if (types[i] == TypePlayer.Chassé) // chassé
                    {
                        indexSpawn = indexSpawnChassé[iChassé];
                        iChassé++;
                    }
                    else if (types[i] == TypePlayer.Blocard) // blocard
                    {
                        indexSpawn = indexSpawnChassé[iChassé];
                        iChassé++;
                    }
                    else
                    {
                        throw new Exception($"Pas encore géré le cas du {types[i]}");
                    }
                    
                    string infoJoueur = PlayerManager.EncodeFormatInfoJoueur(indexSpawn, types[i]);
                    // envoi des infos au concerné(e)
                    Hashtable hash = new Hashtable();
                    hash.Add("InfoCréationJoueur", infoJoueur);
                    PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                }
            }
        }

        private void SendInfoBot()
        {
            // les spawns
            int[] indexSpawnBotRectiligne = CrossManager.Instance.GetSpawnBot();
            //int[] indexSpawnBotRectiligne = ManList.CreateListRange(56, 100);
            int iRectiligne = 0;
            int[] indexSpawnReste = SpawnManager.Instance.GetSpawnBot();
            int iReste = 0;

            // les types en fonction du type de la partie et du nombre de joueur
            TypeBot[] types = typeScene.GetTypeBot();
            int[] nBotParJoueur = ManList.SplitResponsabilité(types.Length, nParticipant);

            int iType = 0;
            for (int iPlayer = 0; iPlayer < nParticipant; iPlayer++)
            {
                // rassembler toutes les infos des bots qu'est responsable le joueur
                int nBot = nBotParJoueur[iPlayer];
                (int indexSpawn, TypeBot type)[] infosBot = new (int, TypeBot)[nBot];

                for (int iBot = 0; iBot < nBot; iBot++, iType++)
                {
                    if (types[iType] == TypeBot.Rectiligne) // rectiligne
                    {
                        infosBot[iBot].indexSpawn = indexSpawnBotRectiligne[iRectiligne];
                        iRectiligne++;
                    }
                    else  // le reste
                    {
                        infosBot[iBot].indexSpawn = indexSpawnReste[iReste];
                        iReste++;
                    }

                    infosBot[iBot].type = types[iType];
                }
                
                if (infosBot.Length == 0)
                    continue; // rien à envoyer

                // envoi des infos au concerné(e)
                string mes = BotManager.EncodeFormatInfoBot(infosBot);
                Hashtable hash = new Hashtable();
                hash.Add("InfoCréationBots", mes);
                PhotonNetwork.PlayerList[iPlayer].SetCustomProperties(hash);
            }
        }

        public void Die(PlayerClass playerClass)
        {
            if (!PlayerManager.Own || PlayerManager.Own.IsQuitting)
            {
                // cela veut dire qu'on est sur un joueur qui a quitté la partie,
                // donc on ne fait rien
                return;
            }
                
            
            if (!players.Contains(playerClass))
            {
                throw new Exception("Un script tente de supprimer un joueur de la liste qui n'y est plus");
            }

            players.Remove(playerClass); // remove de la liste players

            if (playerClass is Chassé)
            {
                // remove de la liste chassés
                chassés.Remove((Chassé) playerClass);

                if (chassés.Count == 0)
                {
                    EndGame(TypePlayer.Chasseur);
                }
            }
            else if (playerClass is Chasseur)
            {
                // remove de la liste chasseurs
                chasseurs.Remove((Chasseur) playerClass);

                if (chasseurs.Count == 0)
                {
                    EndGame(TypePlayer.Chassé);
                }
            }
            else if (playerClass is Blocard)
            {
                // rien, il n'est pas stocké dans une liste spécifique
            }
            else
            {
                throw new Exception($"La mort du type de {playerClass} n'est pas encore implémenté");
            }

            PhotonView pv = playerClass.GetPv(); // on récupère le point de vue du mourant

            if (!pv.IsMine) // Seul le mourant créé un spectateur
                return;

            if (players.Count == 0)
                return;
            
            // création du spectateur
            Spectateur spectateur = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "Spectateur"),
                Vector3.zero, Quaternion.identity, 0, new object[]{pv.ViewID}).GetComponent<Spectateur>();
            
            
            // ajout à la liste 'spectateurs'
            spectateurs.Add(spectateur);
        }

        private void EndGame(TypePlayer typeWinner)
        {
            /*PhotonNetwork.Destroy(ownPlayer.gameObject);
            
            players.Clear();
            chasseurs.Clear();
            chassés.Clear();
            spectateurs.Clear();*/

            _endedGame = true;
            MenuManager.Instance.OpenMenu("EndGame");
            EndGameMenu.Instance.SetWinner(typeWinner);
        }
    }
}