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
    }
}