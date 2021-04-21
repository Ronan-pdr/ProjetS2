using System.Collections;
using System.Collections.Generic;
using Script.InterfaceInGame;
using UnityEngine;

public class LauncherManager : MonoBehaviour
{
    // ------------ SerializedField ------------
    
    [Header("Menus")]
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private TabMenu tabMenu;
    
    // ------------ Constructeur ------------
    void Start()
    {
        PauseMenu.Instance = pauseMenu;
        TabMenu.Instance = tabMenu;
    }
    
    // ------------ Update ------------
    void Update()
    {
        if (pauseMenu.Getdisconnecting())
            return;
        
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
}