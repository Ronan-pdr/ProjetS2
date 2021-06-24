using System;
using System.Collections;
using Photon.Pun;
using Script.Manager;
using Script.Menu;
using UnityEngine;

namespace Script.InterfaceInGame
{
    public class LauncherManager : MonoBehaviour
    {
        // ------------ SerializedField ------------

        [Header("Menus")]
        [SerializeField] private InterfaceInGameManager interfaceInGame;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private TabMenu tabMenu;
        [SerializeField] private EndGameMenu endGameMenu;
        
        // ------------ Attributs ------------

        private MenuManager _menuManager;
        
        // ------------ Constructeur ------------
        private void Awake()
        {
            InterfaceInGameManager.Instance = interfaceInGame;
            PauseMenu.Instance = pauseMenu;
            TabMenu.Instance = tabMenu;
            EndGameMenu.Instance = endGameMenu;
        }

        private void Start()
        {
            _menuManager = MenuManager.Instance;

            if (PhotonNetwork.IsConnected)
            {
                _menuManager.OpenMenu("loading");
            }
        }

        // ------------ Update ------------
        void Update()
        {
            if (pauseMenu.Getdisconnecting())
                return;

            if (MasterManager.Instance.IsGameEnded())
            {
                GestionGameEnded();
            }
            else
            {
                GestionInGame();
            }
        }

        // ------------ Private Méthodes ------------
        
        private void GestionInGame()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // géré le menu pause
                if (pauseMenu.GetIsPaused())
                {
                    pauseMenu.Resume();
                }
                else
                {
                    pauseMenu.Pause();
                }
            }
            else if (!pauseMenu.GetIsPaused())
            {
                // gérer le menu tab
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    // ouvrir menu tab
                    _menuManager.OpenMenu("tab");
                }
                else if (Input.GetKeyUp(KeyCode.Tab))
                {
                    // fermé menu tab -> ouvrir interfaInGame
                    _menuManager.OpenMenu("InterfaceInGame");
                }
            }
        }
        
        private void GestionGameEnded()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // ouvrir de force menu tab pour
                // pas que l'écran de win s'efface
                _menuManager.ForceOpenMenu("tab");
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                // fermé menu tab
                _menuManager.CloseMenu("tab");
            }
        }
    }
}