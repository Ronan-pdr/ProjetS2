using System;
using System.Diagnostics;
using Script.DossierPoint;
using Script.Manager;
using UnityEngine;

namespace Script.MachineLearning
{
    public class EntrainementSaut : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Spawn")]
        [SerializeField] private SpawnPoint point;

        // ------------ Attributs ------------

        private MasterManager _master;
        private Autonome _autonome;
        private Stopwatch _stopwatch;
        private float _score;
        
        // ------------ Getter ------------

        public Autonome Bot => _autonome;
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            _autonome = Instantiate(MasterManager.Instance.GetOriginalAutonome(), Vector3.zero, point.transform.rotation);
            _stopwatch = new Stopwatch();

            _autonome.SetEntrainemetSaut(this);
        }

        private void Start()
        {
            _master = MasterManager.Instance;
            StartEntrainement();
        }

        // ------------ Methods ------------

        private void StartEntrainement()
        {
            // téléportation
            _autonome.transform.position = point.transform.position;
            
            // début du chrono
            _stopwatch.Start();
        }

        public void ResetBot()
        {
            // récupérer le score
            _stopwatch.Stop();
            _score += _stopwatch.ElapsedMilliseconds * 1000;
            
            // 

            // recommencer
            
        }

        public void Malus()
        {
            _score -= 1000;
        }
    }
}