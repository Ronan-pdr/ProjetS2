using System;
using UnityEngine;

namespace Script.MachineLearning
{
    public class EntrainementDeplacement : Entrainement
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
            _terrains[_indexField].BeginTraining(this);
        }

        // ------------ Public Methods ------------
        
        protected override void GetScore()
        {
            // tout est déjà fait dans 'NextField' et dans 'Bonus'
        }

        public override void Bonus()
        {
            Score += 10;
        }

        public override void Malus()
        {
            throw new NotImplementedException();
        }

        public void NextField(bool achieve)
        {
            // récupérer le score :
            // - s'il a réussi, moins il met de temps mieux c'est
            // - s'il n'a pas réussi, plus il est mort vite pire c'est
            Score += _terrains[_indexField].GetScore() * (achieve ? 1 : -1);
            
            // bonus
            if (achieve)
            {
                Bonus();
            }

            // c'est peut-être la fin
            if (_indexField + 1 == _terrains.Length)
            {
                EndTest();
            }
            
            // changer de terrain
            _indexField += 1;
            _terrains[_indexField].BeginTraining(this);

            // téléporter au bon terrain
            _terrains[_indexField].Teleportation(Student.transform);
            
            // indiquer la destination à l'élève
            ((Traqueur) Student).SetGoal(Arrive);
        }

        // ------------ Protected Methods ------------

        protected override Student GetPrefab() => Master.GetOriginalTraqueur();

        protected override void ResetIndicator()
        {}
    }
}