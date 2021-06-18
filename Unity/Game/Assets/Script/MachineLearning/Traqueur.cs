using System;
using System.IO;
using Script.Brain;
using UnityEngine;

namespace Script.MachineLearning
{
    /*public class Traqueur : Student
    {
        // ------------ Attributs ------------

        private EntrainementDeplacement _entrainementDeplacement;
        private Vector3 _destination;

        private NeuralNetwork _knowWhenThereIsObstacle;
        
        // ------------ Setter ------------
        public override void SetToTest()
        {
            running = Running.Marche;
            _destination = _entrainementDeplacement.Arrive;
        }

        // ------------ Methods ------------

        protected override void ErrorEntrainement()
        {
            if (Entrainement is EntrainementDeplacement)
            {
                _entrainementDeplacement = (EntrainementDeplacement)Entrainement;
            }
            else
            {
                throw new Exception();
            }
        }
        
        // ------------ Constructeur ------------

        protected override void AwakeStudent()
        {
            string path = $"Build/{EntrainementDetection.NameDirectory}/0";
            if (File.Exists(path))
            {
                _knowWhenThereIsObstacle = NeuralNetwork.Restore(path);
            }
            else
            {
                throw new Exception($"Path = '{path}'");
            }
        }

        // ------------ Brain ------------

        protected override int[] GetLayerDimension()
        {
            // 6 entrées :
            // - la position de la destination (en z)
            // - la position de la destination (en x)
            // - sa position (en z)
            // - sa position (en x)
            // - sa rotation (en z)
            // - obstacle à 12h (distance)

            // 2 sorties :
            // - avancer
            // - tourner
            
            return new []{6, 2};
        }

        protected override void UseBrain()
        {
            // récupérer les inputs
            
            (_, double height) = GetDistHeightFirstObstacle(Tr.position, MaxDistJump);
            GetResult(_knowWhenThereIsObstacle, new []{height});

            Vector3 ownPos = Tr.position;

            double[] input =
            {
                _destination.z,
                _destination.x,
                ownPos.z,
                ownPos.x,
                Tr.eulerAngles.y,
                
            };
        }
        
        // ------------ Event ------------
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                // il est arrivé
                _entrainementDeplacement.NextField();
            }
        }

        protected override void OnHighCollision(Collision _)
        {
            _entrainementDeplacement.NextField();
        }
    }*/
}