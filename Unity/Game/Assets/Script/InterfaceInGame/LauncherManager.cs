using System.Collections;
using Script.Manager;
using Script.Menu;
using UnityEngine;

namespace Script.InterfaceInGame
{
    public class LauncherManager : MonoBehaviour
    {
        // ------------ SerializedField ------------
    
        [Header("Menus")]
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private TabMenu tabMenu;
        [SerializeField] private EndGameMenu endGameMenu;
    
        // ------------ Attributs------------

        public static LauncherManager Instance;
    
        // ------------ Constructeur ------------
        void Start()
        {
            Instance = this;
        
            PauseMenu.Instance = pauseMenu;
            TabMenu.Instance = tabMenu;
            EndGameMenu.Instance = endGameMenu;
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
                GestioInGame();
            }
            
        }

        // ------------ Méthodes ------------
        private void GestioInGame()
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
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    // ouvrir menu tab
                    MenuManager.Instance.OpenMenu("tab");
                }
                else if (Input.GetKeyUp(KeyCode.Tab))
                {
                    // fermé menu tab -> ouvrir interfaInGame
                    MenuManager.Instance.OpenMenu("InterfaceInGame");
                }
            }
        }
        private void GestionGameEnded()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // ouvrir menu tab
                MenuManager.Instance.ForceOpenMenu("tab");
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                // fermé menu tab -> ouvrir interfaInGame
                MenuManager.Instance.CloseMenu("tab");
            }
        }
    }
}