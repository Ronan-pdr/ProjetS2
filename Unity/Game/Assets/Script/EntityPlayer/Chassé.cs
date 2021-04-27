using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Script.Animation.Personnages.Hunted;
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

        protected override void AwakePlayer()
        {
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutChassé(this);
            
            Anim = GetComponent<HuntedStateAnim>();
        }

        protected override void StartPlayer()
        {
            MaxHealth = 100;
        }
        
        // ------------ Update ------------

        protected override void UpdatePlayer()
        {}
    }
}