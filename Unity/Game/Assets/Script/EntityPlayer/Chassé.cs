using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Script.Bot;
using Script.InterfaceInGame;
using Script.Manager;
using Script.Test;
using Script.Tools;

namespace Script.EntityPlayer
{
    public class Chassé : PlayerClass
    {
        // ------------ Constructeurs ------------
        private void Awake()
        {
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutChassé(this);
            
            AwakePlayer();
        }

        void Start()
        {
            MaxHealth = 100;
            StartPlayer();
        }
        
        // ------------ Update ------------

        protected override void UpdatePlayer()
        {
            AnimationTernier();
        }
    }
}