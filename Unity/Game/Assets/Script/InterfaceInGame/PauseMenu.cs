using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Script.Menu;
using Script.EntityPlayer;
using PlayerManager = Script.EntityPlayer.PlayerManager;

namespace Script.InterfaceInGame
{
    public class PauseMenu : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        public static PauseMenu Instance;
        
        // Etat
        private bool isPaused;
        private bool disconnecting;
        
        // ------------ Getters ------------
        public bool GetIsPaused() => isPaused;
        public bool Getdisconnecting() => disconnecting;
        
        // ------------ Constructeur ------------
        public void Awake()
        {
            isPaused = false;
            disconnecting = false;
        }

        // ------------ Méthodes ------------

        public void Resume()
        {
            MenuManager.Instance.OpenMenu("InterfaceInGame");
            isPaused = false;
        }

        public void Pause()
        {
            MenuManager.Instance.OpenMenu("pause");
            isPaused = true;
        }

        public IEnumerator Quit()
        {
            disconnecting = true;
            PhotonNetwork.Disconnect();
            Destroy(RoomManager.Instance.gameObject);
            
            while(PhotonNetwork.IsConnected)
            {
                yield return null;
            }
            
            SceneManager.LoadScene(1);
        }

        public void StartQuit()
        {
            PlayerManager.Own.BeginToQuit();
            StartCoroutine(Quit());
        }
    }
}