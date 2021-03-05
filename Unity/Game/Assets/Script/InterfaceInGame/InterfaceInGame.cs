using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

namespace Script
{
    public class InterfaceInGame : MonoBehaviourPunCallbacks
    {
        public static InterfaceInGame Instance;
        
        [SerializeField] Transform infoContent;
        [SerializeField] GameObject infoItemPrefab;

        [SerializeField] private AffichageInfoJoueur tesInfos;
        private PlayerClass ownPlayer;
        private List<AffichageInfoJoueur> infosJoueur;
        
        // on attent que le masterManager instancie la liste
        private bool start = false;
        
        // Setter
        public void FeuVert() // est activé lorsque le masterManager a instancié toutes ces listes
        {
            start = true;
        }

        private void Awake()
        {
            Instance = this;
            infosJoueur = new List<AffichageInfoJoueur>();
        }

        private void Update()
        {
            if (start)
                StartFeuVert();
        }
        
        private void StartFeuVert()
        {
            MasterManager masterManager = MasterManager.Instance;
            int hauteur = Screen.height;
            int largeur = Screen.width;
            int taille = 85;
            
            ownPlayer = masterManager.GetOwnPlayer();
            tesInfos.SetUp(ownPlayer, taille, 0);

            int index = 0;
            foreach (PlayerClass player in masterManager.GetListPlayer())
            {
                if (player != ownPlayer)
                {
                    infosJoueur.Add(Instantiate(infoItemPrefab, infoContent).GetComponent<AffichageInfoJoueur>());
                    infosJoueur[index++].SetUp(player, hauteur - index*taille, 90);
                }
            }

            start = false;
        }
    }
}