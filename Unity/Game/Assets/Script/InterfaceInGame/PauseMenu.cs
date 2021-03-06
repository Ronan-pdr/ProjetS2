using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Script
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance;
        
        // Etat
        private bool isPaused;
        private bool disconnecting;
        
        // les interfaces
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject InterfaceInGame; 
        
        // Getter
        public bool GetIsPaused() => isPaused;
        
        public void Awake()
        {
            Instance = this;
            
            isPaused = false;
            disconnecting = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (disconnecting)
                return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            isPaused = false;

            InterfaceInGame.SetActive(true);
        }

        void Pause()
        {
            pauseMenuUI.SetActive(true);
            isPaused = true;
            
            InterfaceInGame.SetActive(false);
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