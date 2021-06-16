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
        private int _score;
        private ClassementSaut _classementSaut;
        
        // ------------ Getter ------------

        public Autonome Bot => _autonome;
        
        // ------------ Setter ------------

        public void SetClassement(ClassementSaut value)
        {
            _classementSaut = value;
        }
        
        // ------------ Constructeur ------------

        private void Awake()
        {}

        private void Start()
        {
            _master = MasterManager.Instance;
            
            _autonome = Instantiate(_master.GetOriginalAutonome(), Vector3.zero, point.transform.rotation);
            _stopwatch = new Stopwatch();

            _autonome.SetEntrainemetSaut(this);
            StartEntrainement();
        }

        // ------------ Methods ------------

        private void StartEntrainement()
        {
            // téléportation
            _autonome.transform.position = point.transform.position;
            
            // reset score
            _score = 0;
            
            // début du chrono
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void ResetBot()
        {
            // récupérer le score
            _stopwatch.Stop();
            _score += (int)_stopwatch.ElapsedMilliseconds;
            
            // le donner au classement
            _autonome.SetNeurone(_classementSaut.EndRace(_autonome.Neurones, _score));

            // recommencer
            StartEntrainement();
        }

        public void Malus()
        {
            _score -= 1000;
        }
    }
}