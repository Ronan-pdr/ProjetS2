using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Script.EntityPlayer;
using Script.Manager;
using Image = UnityEngine.UI.Image;

namespace Script.InterfaceInGame
{
    public class InterfaceInGameManager : MonoBehaviourPunCallbacks
    {
        // ------------ SerializedField ------------
        
        [Header("Prefab")]
        [SerializeField] private AffichageInfoJoueur tesInfos;

        // ------------ Attributs ------------
        
        public static InterfaceInGameManager Instance;

        // relatif à toi
        private PlayerClass ownPlayerClass;

        // ------------ Constructeur ------------
        private void Awake()
        {
            Instance = this;
        }
        
        // ------------ Méthodes ------------

        public void Set()
        {
            MasterManager masterManager = MasterManager.Instance;
            int hauteur = Screen.height;
            //int largeur = Screen.width;
            int taille = 85;
            
            ownPlayerClass = masterManager.GetOwnPlayer();
            tesInfos.SetUp(ownPlayerClass, 80, -25);
        }
    }
}