using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.PlayerLoop;

namespace Script
{
    public class Chassé : PlayerClass
    {
        private void Awake()
        {
            AwakePlayer();
        }

        void Start()
        {
            StartPlayer();
        }
        
        void Update()
        {
            UpdateHumanoide();
            UpdatePlayer();
        }

        private void FixedUpdate()
        {
            FixedUpdatePlayer();
        }


        // Animation
        

        private static void Search()

        protected override void AnimationContinue(string touche)
        {

        }

        protected override void AnimationAcoup(string touche)
        {

        }



    }
}