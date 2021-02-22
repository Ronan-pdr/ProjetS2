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
            Awa();
        }

        void Start()
        {
            Sta();
        }
        
        void Update()
        {
            Upd();
        }

        private void FixedUpdate()
        {
            FixedUpd();
        }
    }
}