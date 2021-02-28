using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Script.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool isPaused = false;
        private bool disconnecting = false;
        public GameObject pauseMenuUI;

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