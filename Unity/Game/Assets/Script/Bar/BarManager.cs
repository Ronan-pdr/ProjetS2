using System;
using System.Collections.Generic;
using Photon.Pun;
using Script.EntityPlayer;
using Script.InterfaceInGame;
using Script.Menu;
using UnityEngine;
using Random = System.Random;

namespace Script.Bar
{
    public class BarManager : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Menu")]
        [SerializeField] private Menu.Menu menuWaiting;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private MenuTabBar tabMenu;

        [Header("Spawner")]
        [SerializeField] private Transform[] spawns;
        
        // ------------ Attributs ------------

        public static BarManager Instance;
        private Random _rnd;
        
        // ------------ Getter ------------

        public Transform GetSpawn()
        {
            return spawns[_rnd.Next(spawns.Length)];
        }
        
        // ------------ Setter ------------

        public void NewHunted(Chassé value)
        {
            // enlever son coeur
            value.GetComponentInChildren<HumanHeart>().gameObject.SetActive(false);
        }
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            // s'initialiser et les autres
            Instance = this;
            PauseMenu.Instance = pauseMenu;
            
            // initialiser le reste
            _rnd = new Random();
        }

        private void Start()
        {
            // afficher le bon menu
            MenuManager.Instance.OpenMenu(menuWaiting);
        }

        // ------------ Update ------------

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenu.GetIsPaused())
                {
                    // fermer le menu escape
                    pauseMenu.Resume();
                }
                else
                {
                    // ouvrir le menu escape
                    pauseMenu.Pause();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                // ouvrir le menu tab
                tabMenu.Open();
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                // fermer le menu tab
                MenuManager.Instance.OpenMenu(menuWaiting);
            }
        }
    }
}