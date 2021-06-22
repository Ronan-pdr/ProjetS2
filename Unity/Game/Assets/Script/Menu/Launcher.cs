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
        // ------------ SerializeField ------------
        
        [SerializeField] TMP_InputField roomNameInputField;
        [SerializeField] TMP_Text errorText;
        //Liste des rooms disponibles
        [SerializeField] Transform roomListContent;
        //Liste des joueurs dans la room
        [SerializeField] GameObject roomListItemPrefab;
        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button findRoomButton;
        [SerializeField] private AudioManager audioManager;
        
        // ------------ Attributs ------------
        
        public static Launcher Instance;
        private const string PlayerPrefsNameKey = "PlayerName";

        // ------------ Constructeur ------------
        
        void Awake()
        {
            new TouchesClass();
            Instance = this;
        }
        
        //Se connecte au serveur que l'on retrouve dans Assets/Photon/Photon/UnityNetworking/Ressources/PhotonSer...
        private void Start()
        {
            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
            SetUpInputField();
            audioManager.audioSource.volume = PlayerPrefs.GetFloat("volumeMenu", 30f*0.15f/100f);
        }
    
        // ------------ Connexion ------------
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
        
        // ------------ Créer/Rejoindre ------------
        
        //Est appelé par un boutton
        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
                return;

            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu("loading");
            SavePlayerName();
        }
        
        //Est appelé par un boutton
        public void JoinRoom(RoomInfo info)
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("loading");
            SavePlayerName();
        }
        
        //Est appelé automatiquement après 'Join Room'
        public override void OnJoinedRoom()
        {
            // c'est parti pour le bar
            PhotonNetwork.LoadLevel(1);
        }
        
        // ------------ Error ------------
    
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed" + message;
            MenuManager.Instance.OpenMenu("error");
        }

        // ------------ Liste des chambres ------------

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
        
        // ------------ Names ------------

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsNameKey))
                return;
        
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = defaultName;
            SetPlayerName();
        }

        private void SetPlayerName()
        {
            createRoomButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
            findRoomButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
        }

        private void SavePlayerName()
        {
            string playerName = nameInputField.text;
            PhotonNetwork.NickName = playerName;
            PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
        }
        
        // ------------ Quitter ------------

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
