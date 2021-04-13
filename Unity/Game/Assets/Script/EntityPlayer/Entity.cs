using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Script.Bot;
using Script.TeteChercheuse;

namespace Script.EntityPlayer
{
    public class Entity : MonoBehaviourPunCallbacks
    {
        protected Transform Tr;
        protected Rigidbody Rb;

        //Déplacement
        protected Vector3 MoveAmount;
        private Vector3 smoothMoveVelocity;
        private float smouthTime = 0.15f;
        
        // masterManager
        protected MasterManager master;
        
        // Getter
        public string GetTypeEntity()
        {
            string type;
            
            if (this is BotClass)
                type = "Bot";
            else if (this is Chasseur)
                type = "Chasseur";
            else if (this is Chassé)
                type = "Chassé";
            else if (this is BodyChercheur)
                type = "BodyChercheur";
            else if (this is BalleFusil)
                type = "BallFusil";
            else
                type = "Unrepetoried";

            return type;
        }

        private void Start()
        {
            master = MasterManager.Instance;
        }

        //Setter
        protected void SetRbTr()
        {
            Rb = GetComponent<Rigidbody>();
            Tr = GetComponent<Transform>();
        }

        protected void SetMoveAmount(Vector3 moveDir, float speed) // moveDir doit être de la forme (1, 0, 0), (0, 0, -1), (1, 0, 1)... mais pas de 1 sur y (pour les humains du moins)
        {
            MoveAmount = Vector3.SmoothDamp(MoveAmount,
                moveDir * speed,
                ref smoothMoveVelocity, smouthTime);
        }
    
        protected void MoveEntity()
        {
            
            //Déplace le corps du human grâce à moveAmount précédemment calculé
            Rb.MovePosition(Rb.position + Tr.TransformDirection(MoveAmount) * Time.fixedDeltaTime);
        }

        public override string ToString()
        {
            string type = GetTypeEntity();
            
            if (this is Humanoide)
            {
                return $"{type}[{name}]";
            }

            return $"{type}";
        }
    }
}