using System;
using System.Collections.Generic;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class LabyrintheManager : MonoBehaviour
    {
        // ------------ Serialized Field ------------
        
        [Header("Sortie")]
        [SerializeField] private GameObject sortie;
        
        // ------------ Attributs ------------
        
        public static LabyrintheManager Instance;
        private RayGaz sonde;
        private bool isSondeFinish;
        
        // ------------ Setter ------------

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
            //sonde = RayGaz.GetSonde(sortie.transform.position, FinSonde);
        }
        
        // ------------ Méthode ------------
        public List<Vector3> GetBestPath(Vector3 pos)
        {
            if (isSondeFinish)
            {
                return sonde.GetBestPath(pos);
            }

            return new List<Vector3>();
        }
    }
}