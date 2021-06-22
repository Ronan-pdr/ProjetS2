using System;
using System.Collections.Generic;
using Photon.Pun;
using Script.EntityPlayer;
using UnityEngine;
using Random = System.Random;

namespace Script.Manager
{
    public class BarManager : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [SerializeField] private Transform[] spawns;
        
        // ------------ Attributs ------------

        public static BarManager Instance;

        private List<Chassé> _chassés;
        private Random _rnd;
        
        // ------------ Getter ------------

        public Transform GetSpawn()
        {
            return spawns[_rnd.Next(spawns.Length)];
        }
        
        // ------------ Setter ------------

        public void AddHunted(Chassé value)
        {
            _chassés.Add(value);
        }
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            // s'initialiser
            Instance = this;
            
            // initialiser le reste
            _chassés = new List<Chassé>();
            _rnd = new Random();
        }
    }
}