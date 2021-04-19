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
        // constructeurs
        private void Awake()
        {
            AwakePlayer();
            
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutChassé(this);
        }

        void Start()
        {
            MaxHealth = 100;
            StartPlayer();
        }
        
        void Update()
        {
            if (!Pv.IsMine)
                return;
            
            Cursor.lockState = PauseMenu.Instance.GetIsPaused() ? CursorLockMode.None : CursorLockMode.Confined;
            Cursor.visible = PauseMenu.Instance.GetIsPaused();
            
            if (PauseMenu.Instance.GetIsPaused())
            {
                MoveAmount = Vector3.zero;
                return;
            }

            UpdatePlayer();
            Animation();
        }

        private void FixedUpdate()
        {
            FixedUpdatePlayer();
        }
        
        //GamePlay

        protected override void Die() // Est appelé lorsqu'il vient de mourir
        {
            MasterManager.Instance.Die(Pv.Owner);
            
            anim.enabled = false;
            PhotonNetwork.Destroy(gameObject);
        }
    }
}