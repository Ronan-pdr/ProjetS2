using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Script.EntityPlayer;
using Script.Manager;
using Script.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.InterfaceInGame
{
    public class EndGameMenu : MonoBehaviourPunCallbacks
    {
        // ------------ Serialize Field ------------

        [Header("Restart")]
        [SerializeField] private GameObject reStartGameButton;

        [Header("Ecran D'affichage")]
        [SerializeField] private GameObject ecranWin;
        [SerializeField] private GameObject ecranLose;
        
        // ------------ Attributs ------------

        public static EndGameMenu Instance;

        private TypePlayer _winner;
        
        // ------------ Setters ------------

        public void SetWinner(TypePlayer winner)
        {
            _winner = winner;
        }
        
        // ------------ Constructeurs ------------

        private void Start()
        {
            // Eécran win ou écran lose
            if (PlayerManager.Own.Type == _winner)
            {
                ecranWin.SetActive(true);
            }
            else
            {
                ecranLose.SetActive(true);
            }
            
            // Le masterClient a le pouvoir de restart
            reStartGameButton.SetActive(PhotonNetwork.IsMasterClient);
            
            // Gérer la souris
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ------------ On Button Click ------------

        public void Restart()
        {
            // c'est parti pour le bar
            PhotonNetwork.LoadLevel(1);
        }
        
        // ------------ Event ------------
        
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            reStartGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
}
