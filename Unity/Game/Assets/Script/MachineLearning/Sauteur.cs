using System;
using Script.Animation;
using Script.Bot;
using Script.Graph;
using Script.Test;
using UnityEngine;
using Random = System.Random;

namespace Script.MachineLearning
{
    public class Sauteur : Student
    {
        // ------------ Setter ------------
        public override void SetToTest()
        {
            running = Running.Marche;
        }

        // ------------ Methods ------------

        protected override void ErrorEntrainement()
        {
            if (!(Entrainement is EntrainementSaut))
            {
                throw new Exception();
            }
        }
        
        // ------------ Constructeur ------------
        
        protected override void AwakeStudent()
        {}

        // ------------ Brain ------------

        protected override int[] GetLayerDimension()
        {
            // 3 entrées :
            // - la distance entre le bot et l'obstacle
            // - la hauteur du premier obstacle
            // - la vitesse du bot
            
            // 1 sortie :
            // - should jump
            
            return new []{3, 1};
        }

        protected override void UseBrain()
        {
            if (!Grounded)
                return;
            
            // recupérer les infos par rapport aux obstacles
            (double minDist, double height) = GetDistHeightFirstObstacle(Tr.position, MaxDistJump);
            
            // input correspondant à cette tâche
            double[] input = InputJump(minDist, height);
            
            ErrorInput(input);

            // output du neurones
            double[] output = GetResult(Neurones, input);
            
            // reaction
            if (output[0] > 0.5f)
            {
                Jump();
                Entrainement.Malus();
            }
        }
        
        // ------------ Event ------------
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("SetRun"))
            {
                running = Running.Course;
            }
        }

        protected override void OnHighCollision(Collision _)
        {
            Entrainement.EndTest();
        }
    }
}