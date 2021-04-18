using System;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class LabyrintheManager : MonoBehaviour
    {
        // attributs
        public LabyrintheManager Instance;
        
        private RayGaz.Node[,] Sonde;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
        }
    }
}