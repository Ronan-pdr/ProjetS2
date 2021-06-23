using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Menu;
using UnityEngine;

namespace Script.Bar
{
    public class EscapeBarMenu : MonoBehaviourPunCallbacks
    {
        // ------------ SerializeField ------------
    
        [Header("Boutton")]
        [SerializeField] private GameObject gameSettingsButton;
        [SerializeField] private GameObject startGameButton;
        
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            SetPrivilegeMaster();
        }

        // ------------ Public Methodes ------------

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            SetPrivilegeMaster();   
        }
    
        public void StartGame()
        {
            if (PhotonNetwork.MasterClient.NickName == "Labyrinthe")
            {
                PhotonNetwork.LoadLevel(3);
            }
            else
            {
                PhotonNetwork.LoadLevel(2);
            }
        }
    
        // Est appelé par un boutton
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        
            // c'est reparti pour le menu
            PhotonNetwork.LoadLevel(0);
        }
    
        // ------------ Private Methodes ------------

        private void SetPrivilegeMaster()
        {
            gameSettingsButton.SetActive(PhotonNetwork.IsMasterClient);
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
}
