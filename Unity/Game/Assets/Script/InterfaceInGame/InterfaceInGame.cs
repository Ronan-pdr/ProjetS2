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
        
        // pour instancier sur la toile l'affichage du joueur
        [SerializeField] Transform infoContent;
        [SerializeField] GameObject infoItemPrefab;

        // relatif à toi
        [SerializeField] private AffichageInfoJoueur tesInfos;
        private PlayerClass ownPlayerClass;
        
        // relatif aux autres
        private List<AffichageInfoJoueur> infosJoueur;

        private void Awake()
        {
            Instance = this;
            infosJoueur = new List<AffichageInfoJoueur>();
        }
        
        public void Set()
        {
            MasterManager masterManager = MasterManager.Instance;
            int hauteur = Screen.height;
            int largeur = Screen.width;
            int taille = 85;
            
            ownPlayerClass = masterManager.GetOwnPlayer();

            tesInfos.SetUp(ownPlayerClass, taille, 0);

            int index = 0;
            foreach (PlayerClass playerClass in masterManager.GetListPlayer()) // pas grave si la liste est vide (mais elle est forcément instancié)
            {
                if (playerClass != ownPlayerClass)
                {
                    infosJoueur.Add(Instantiate(infoItemPrefab, infoContent).GetComponent<AffichageInfoJoueur>());
                    infosJoueur[index++].SetUp(playerClass, hauteur - index*taille, 90);
                }
            }
        }
    }
}