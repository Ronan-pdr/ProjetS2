using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Script.EntityPlayer;
using Script.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.InterfaceInGame
{
    public class EndGameMenu : MonoBehaviourPunCallbacks
    {
        // ------------ Serialize Field ------------

        [Header("Restart")]
        [SerializeField] private GameObject restartGameButton;

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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        
            if (PlayerManager.Own.Type == _winner)
            {
                ecranWin.SetActive(true);
            }
            else
            {
                ecranLose.SetActive(true);
            }
            
            restartGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
        
        // ------------ Méthodes ------------
        
        public IEnumerator Quit()
        {
            PhotonNetwork.Disconnect();
            Destroy(RoomManager.Instance.gameObject);
            
            while(PhotonNetwork.IsConnected)
            {
                yield return null;
            }
            
            SceneManager.LoadScene(0);
        }

        // ------------ On Button Click ------------
        
        public void StartQuit()
        {
            StartCoroutine(Quit());
        }

        public void Restart()
        {
            
        }
        
        // ------------ Event ------------
        
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            restartGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
}
