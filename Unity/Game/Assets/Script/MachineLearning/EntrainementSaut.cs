using System;
using System.Diagnostics;
using Script.DossierPoint;
using Script.Manager;
using Script.Tools;
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
        private Sauteur _sauteur;
        private int _score;
        private ClassementDarwin _classement;
        private int _nSaut;
        
        // ------------ Getter ------------

        public Sauteur Bot => _sauteur;
        
        // ------------ Setter ------------

        public void SetClassement(ClassementDarwin value)
        {
            _classement = value;
        }
        
        // ------------ Constructeur ------------

        private void Start()
        {
            _master = MasterManager.Instance;
            
            _sauteur = Instantiate(_master.GetOriginalSauteur(), Vector3.zero, point.transform.rotation);

            _sauteur.SetEntrainementSaut(this);
            StartEntrainement();
        }

        // ------------ Public Methods ------------
        
        public void ResetBot()
        {
            // récupérer le score
            
            // bonus
            float dist = Calcul.Distance(_sauteur.transform.position, point.transform.position);
            _score += (int)(dist * CoefScore);
            
            if (dist > 5)
            {
                // malus
                _score -= _nSaut * CoefScore;
            }
            
            // le donner au classement
            _sauteur.SetNeurone(_classement.EndEpreuve(_sauteur.Neurones, _score));

            // recommencer
            StartEntrainement();
        }

        public void Malus()
        {
            _nSaut += 1;
        }
        
        // ------------ Private Methods ------------

        private void StartEntrainement()
        {
            // téléportation
            _sauteur.transform.position = point.transform.position;
            
            // reset score et indicateurs
            _score = 0;
            _nSaut = 0;
            
            // set le bot
            _sauteur.SetToRace();
        }

        private int CoefScore => 10;
    }
}