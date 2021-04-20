using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

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
            SceneManager.LoadScene(0);
        }

        public void StartQuit()
        {
            StartCoroutine(Quit());
        }
    }
}