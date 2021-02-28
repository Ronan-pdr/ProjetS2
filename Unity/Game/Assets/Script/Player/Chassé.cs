using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.PlayerLoop;

namespace Script
{
    public class Chassé : PlayerClass
    {
        /*private string _touchLeverAssoir = "c";
        private bool _debout;
        private double previousTime;*/
        
        private void Awake()
        {
            AwakePlayer();
            InitialiserAnimationVariable();
        }

        void Start()
        {
            StartPlayer();
        }
        
        void Update()
        {
            if (PauseMenu.PauseMenu.isPaused)
            {
                moveAmount = Vector3.zero;
                return;
            }
            
            UpdateHumanoide();
            UpdatePlayer();
        }

        private void FixedUpdate()
        {
            FixedUpdatePlayer();
        }


        // Animation
        private (string, string)[] arrAnim; // (touche, animation)
        
        private void InitialiserAnimationVariable()
        {
            arrAnim = new (string, string)[6];
            arrAnim[0] = (touchAvancer, "Avant");
            arrAnim[1] = (touchSprint, "Course");
            arrAnim[2] = (touchReculer, "Arriere");
            arrAnim[3] = (touchDroite, "Droite");
            arrAnim[4] = (touchGauche, "Gauche");
            arrAnim[5] = (touchJump, "Jump");
        }

        protected override void SearchAnimation(string touche)
        {
            int t = arrAnim.Length;
            
            int i;
            for (i = 0; i < t && arrAnim[i].Item1 != touche; i++)
            {}

            if (i == t)
                return;

            anim.enabled = true;
            anim.Play(arrAnim[i].Item2);
        }
    }
}