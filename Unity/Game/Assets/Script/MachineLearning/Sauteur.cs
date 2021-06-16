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
        // ------------ Attributs ------------
        
        private const float MaxDist = 3;

        // ------------ Update ------------

        protected override void UpdateBot()
        {}
        
        // ------------ Methods ------------

        protected override void ErrorEntrainement()
        {
            if (!(Entrainement is EntrainementSaut))
            {
                throw new Exception();
            }
        }

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

            Vector3 pos = Tr.position;

            (float minDist, float height) = GetDistHeightFirstObstacle(pos, MaxDist);

            double[] input =
            {
                minDist / MaxDist, height / capsule.Height, WalkSpeed / SprintSpeed
            };

            for (int i = 0; i < 3; i++)
            {
                if (input[i] < -0.1 || input[i] > 1.1)
                {
                    Debug.Log($"input[{i}] = {input[i]}");
                }
            }

            // feed et enclencher les neurones
            Neurones.Feed(input);
            Neurones.FrontProp();
            
            // récupérer le résultat
            double[] values = Neurones.GetResult();

            if (values[0] > 0.5f)
            {
                Jump();
                Entrainement.Malus();
            }
        }
    }
}