using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Script.MachineLearning
{
    public class TerrainDeplacement : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [SerializeField] private Transform depart;
        [SerializeField] private Transform arrive;
        
        // ------------ Attributs ------------

        // indicateur
        private const int TimeMax = 30;
        private Stopwatch _chrono;
        
        // student
        //private

        // ------------ Getter ------------

        public int GetScore()
        {
            _chrono.Stop();

            return (int)(_chrono.ElapsedMilliseconds / 1000) / TimeMax * 100;
        }

        public Vector3 Arrive => arrive.position;
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            _chrono = new Stopwatch();
        }
        
        // ------------ Methods ------------

        public void Teleportation(Transform tr)
        {
            tr.position = depart.position;
            tr.rotation = arrive.rotation;
        }
    }
}