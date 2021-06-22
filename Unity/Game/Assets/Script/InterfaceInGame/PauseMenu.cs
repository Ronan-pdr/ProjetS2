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
        [Header("Precision")]
        [SerializeField] private Menu.Menu menuToOpenWhenResume;
        
        // ------------ Attributs ------------
        
        public static PauseMenu Instance;
        
        // Etat
        private bool _isPaused;
        private bool disconnecting;
        
        // ------------ Getters ------------
        public bool GetIsPaused() => _isPaused;
        public bool Getdisconnecting() => disconnecting;
        
        // ------------ Constructeur ------------
        public void Awake()
        {
            _isPaused = false;
            disconnecting = false;
        }

        // ------------ Public Méthodes ------------

        public void Resume()
        {
            MenuManager.Instance.OpenMenu(menuToOpenWhenResume);
            _isPaused = false;
        }

        public void Pause()
        {
            MenuManager.Instance.OpenMenu("pause");
            _isPaused = true;
        }
        
        public void StartQuit()
        {
            disconnecting = true;
            
            PlayerManager.Own.BeginToQuit();
            StartCoroutine(Quit());
        }
        
        // ------------ Private Méthodes ------------

        private IEnumerator Quit()
        {
            PhotonNetwork.Disconnect();
            Destroy(RoomManager.Instance.gameObject);

            while(PhotonNetwork.IsConnected)
            {
                yield return null;
            }
            
            SceneManager.LoadScene(0);
        }
    }
}