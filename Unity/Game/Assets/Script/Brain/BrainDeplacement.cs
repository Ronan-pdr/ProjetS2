using UnityEngine;

namespace Script.Brain
{
    public class BrainDeplacement : BrainClass
    {
        // ------------ Attributs ------------

        public enum Output
        {
            Avancer = 0,
            TournerHoraire = 1,
            TournerTrigo = 2
        }
        
        // sauvegarde
        public const string NameDirectory = "SauvegardeNeuroneDeplacement";
        
        // brain
        private BrainWall _brainWall;

        // ------------ Getter ------------

        protected override string GetNameDirectory() => NameDirectory;
        
        // ------------ Constructeur ------------

        public BrainDeplacement()
        {
            Set();
            _brainWall = new BrainWall(0);
        }

        public BrainDeplacement(int numero)
        {
            Set(numero);
            _brainWall = new BrainWall(0);
        }
        
        protected override void NewNeuralNetwork()
        {
            // 6 entrées :
            // - la position de la destination (en z)
            // - la position de la destination (en x)
            // - sa position (en z)
            // - sa position (en x)
            // - sa rotation (en z)
            // - obstacle à 12h (booléen)

            // 3 sorties :
            // - avancer
            // - tourner sens horaire
            // - tourner sens trigo

            Neurones = new NeuralNetwork(new []{6, 2});
        }
        
        // ------------ Methods ------------

        public Output WhatDeplacementShouldDo(Transform tr, Vector3 destination)
        {
            bool thereIsObstacle = _brainWall.IsThereWall(tr, 4); 
            
            return WhatDeplacementShouldDo(tr, destination, thereIsObstacle);
        }
        
        public Output WhatDeplacementShouldDo(Transform tr, Vector3 destination, bool obstacle)
        {
            Vector3 ownPos = tr.position;
            
            // former l'input
            double[] input = {destination.z, destination.x, ownPos.z, ownPos.x, tr.rotation.z, BoolToInt(obstacle)};
            
            // vérifier que toutes les valeurs sont entre -1 et 1
            ErrorInput(input);
            
            // récupérer l'output
            double[] output = GetResult(Neurones, input);
            
            // interpréter l'output
            return (Output)Max(output);
        }
    }
}