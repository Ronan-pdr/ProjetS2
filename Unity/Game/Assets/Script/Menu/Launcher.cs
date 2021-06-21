using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Script.EntityPlayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Menu
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public static Launcher Instance;
        [SerializeField] TMP_InputField roomNameInputField;
        [SerializeField] TMP_Text errorText;
        [SerializeField] TMP_Text roomNameText;
        //Liste des rooms disponibles
        [SerializeField] Transform roomListContent;
        //Liste des joueurs dans la room
        [SerializeField] Transform playerListContent;
        [SerializeField] GameObject roomListItemPrefab;
        [SerializeField] GameObject PlayerListItemPrefab;
        [SerializeField] GameObject startGameButton;
        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button findRoomButton;
        [SerializeField] private AudioManager audioManager;

        private const string PlayerPrefsNameKey = "PlayerName";

        void Awake()
        {
            new TouchesClass();
            Instance = this;
        }
        
        //Se connecte au serveur que l'on retrouve dans Assets/Photon/Photon/UnityNetworking/Ressources/PhotonSer...
        void Start()
        {
            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
            SetUpInputField();
            audioManager.audioSource.volume = PlayerPrefs.GetFloat("volumeMenu", 30f*0.15f/100f);
        }
    
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("title");
            Debug.Log("Joined Lobby");
            SavePlayerName();
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
                return;

            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu("loading");
            SavePlayerName();
        }
        
        //Est appelé automatiquement après 'Join Room'
        public override void OnJoinedRoom()
        {
            MenuManager.Instance.OpenMenu("room");
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            Player[] players = PhotonNetwork.PlayerList;
            
            //Détruit tous les joueurs précédements enregistré dans la room
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }
            
            ChangeName(players[players.Length-1]);

            foreach (Player player in players)
            {
                Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
            }

            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
        
        //Est appelé lorsque le créateur sort de la room, le but de son contenu est d'aficher le bouton 'start' au nouveau master
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed" + message;
            MenuManager.Instance.OpenMenu("error");
        }
    
        public void StartGame()
        {
            PhotonNetwork.LoadLevel(PhotonNetwork.MasterClient.NickName == "Labyrinthe" ? 2 : 1);
        }
    
        //Est appelé par un boutton
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu("loading");
        }
    
        //Est appelé par un boutton
        public void JoinRoom(RoomInfo info)
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("loading");
            SavePlayerName();
        }
    
        //Est appelé automatiquement après 'Leave Room'
        public override void OnLeftRoom()
        {
            MenuManager.Instance.OpenMenu("title");
        }

        //Est appelé automatiquement dés que y'a un changement dans la liste des rooms
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (Transform trans in roomListContent)
            {
                Destroy(trans.gameObject);
            }

            foreach (RoomInfo room in roomList)
            {
                //Unity ne supprime pas une room vide ou pleine ou caché, elle met juste l'attribut RemovedF... = true
                if (room.RemovedFromList)
                    continue;
                
                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
            }
        }
        
        //Est appelé automatiquement losque qu'un joueur rentre dans la room
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ChangeName(newPlayer);
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        }

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsNameKey))
                return;
        
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = defaultName;
            SetPlayerName(defaultName);
        }

        public void SetPlayerName(string name)
        {
            createRoomButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
            findRoomButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
        }

        public void SavePlayerName()
        {
            string playerName = nameInputField.text;
            PhotonNetwork.NickName = playerName;
            PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void ChangeName(Player newPlayer)
        {
            string nickName = newPlayer.NickName;
            Player[] players = PhotonNetwork.PlayerListOthers;
            int count = 0;
            for (int i = 0; i < players.Length;i++)
            {
                if (players[i].Equals(newPlayer))
                    continue;
                if (players[i].NickName == newPlayer.NickName)
                {
                    count += 1;
                    newPlayer.NickName = nickName + count;
                }
            }
        }
    }
}
