using System;
using System.Collections.Generic;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class LabyrintheManager : MonoBehaviour
    {
        // ------------ Serialized Field ------------
        [SerializeField] private Vector3 Sortie;
        
        // ------------ Attributs ------------
        
        public LabyrintheManager Instance;
        private RayGaz sonde;
        private bool isSondeFinish;
        
        // ------------ Setters ------------

        private void FinSonde()
        {
            isSondeFinish = true;
        }

        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Sonder la zone une bonne fois pour toute
            sonde = RayGaz.GetSonde(Sortie, FinSonde);
        }
        
        // ------------ Méthodes ------------
        public List<Vector3> GetBestPath(Vector3 pos)
        {
            return sonde.GetBestPath(pos);
        }
    }
}