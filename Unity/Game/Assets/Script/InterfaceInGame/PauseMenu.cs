using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Script.InterfaceInGame
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance;
        
        // Etat
        private bool isPaused;
        private bool disconnecting;
        
        
        // Getter
        public bool GetIsPaused() => isPaused;
        public bool Getdisconnecting() => disconnecting;
        
        public void Awake()
        {
            Instance = this;
            
            isPaused = false;
            disconnecting = false;
        }

        // Update is called once per frame
        

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