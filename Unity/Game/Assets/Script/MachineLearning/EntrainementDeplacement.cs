using System;
using System.Diagnostics;
using Script.Tools;
using Script.Brain;
using UnityEngine;

namespace Script.MachineLearning
{
    /*public class EntrainementDeplacement : Entrainement
    {
        // ------------ Attributs ------------

        public const string NameDirectory = "SauvegardeNeuroneSaut";

        // terrains
        private TerrainDeplacement[] _terrains;
        private int _indexField;
        
        // ------------ Getter ------------
        public override string GetNameDirectory() => NameDirectory;

        public Vector3 Arrive => _terrains[_indexField].Arrive;
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            _terrains = GetComponentsInChildren<TerrainDeplacement>();
            _indexField = 0;
        }

        // ------------ Public Methods ------------
        
        protected override void GetScore()
        {
            // tout est déjà fait dans 'NextField'
        }

        public override void Malus()
        {
            throw new NotImplementedException();
        }

        public void NextField()
        {
            // récupérer le score
            Score += _terrains[_indexField].GetScore();
            
            // c'est peut-être la fin
            if (_indexField + 1 == _terrains.Length)
            {
                EndTest();
            }
            
            // changer de terrain
            _indexField += 1;

            // téléporter au bon terrain
            _terrains[_indexField].Teleportation(Student.transform);
        }

        // ------------ Protected Methods ------------

        protected override Student GetPrefab() => Master.GetOriginalSauteur();

        protected override void ResetIndicator()
        {}
    }*/
}