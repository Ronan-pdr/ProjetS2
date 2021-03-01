using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool isPaused;
        private bool disconnecting;
        public GameObject pauseMenuUI;

        public void Start()
        {
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
        }

        void Pause()
        {
            pauseMenuUI.SetActive(true);
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