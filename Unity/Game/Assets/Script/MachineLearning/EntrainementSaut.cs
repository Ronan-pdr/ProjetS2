using System;
using System.Diagnostics;
using Script.DossierPoint;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.MachineLearning
{
    public class EntrainementSaut : Entrainement
    {
        // ------------ Attributs ------------

        private int _nSaut;

        // ------------ Public Methods ------------
        
        public override void EndTest()
        {
            // récupérer le score
            
            // bonus
            float dist = Calcul.Distance(Student.transform.position, begin.position);
            Score += (int)(dist * CoefScore);
            
            if (dist > 5)
            {
                // malus
                Score -= _nSaut * CoefScore;
            }
            
            // le donner au classement
            Student.SetNeurone(Classement.EndEpreuve(Student.NeuralNetwork, Score));

            // recommencer
            StartEntrainement();
        }

        public override void Malus()
        {
            _nSaut += 1;
        }
        
        // ------------ Protected Methods ------------

        protected override Student GetPrefab() => Master.GetOriginalSauteur();

        protected override void ResetIndicator()
        {
            _nSaut = 0;
        }

        // ------------ Private Methods ------------

        private int CoefScore => 10;
    }
}