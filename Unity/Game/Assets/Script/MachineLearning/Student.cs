using System;
using Script.Animation;
using Script.Bot;
using UnityEngine;

namespace Script.MachineLearning
{
    public abstract class Student : BotClass
    {
        // ------------ Attributs ------------
        
        protected NeuralNetwork Neurones;
        protected Entrainement Entrainement;
        
        protected const float MaxDistJump = 3;
        
        // ------------ Getter ------------
        
        public NeuralNetwork NeuralNetwork => Neurones;

        // ------------ Setter ------------
        
        public void SetNeurone(NeuralNetwork value)
        {
            Neurones = value;
        }

        public void SetEntrainement(Entrainement value)
        {
            Entrainement = value;
            
            ErrorEntrainement();
        }
        
        // ------------ Constructeur ------------

        protected abstract void AwakeStudent();

        protected override void AwakeBot()
        {
            Neurones = new NeuralNetwork(GetLayerDimension());
            AwakeStudent();
        }
        
        protected override void StartBot()
        {
            SetToTest();
            InvokeRepeating(nameof(UseBrain), 0.1f, 0.1f);
            //Invoke(nameof(PotentielJump), 0.5f);
        }
        
        // ------------ Update ------------

        protected override void UpdateBot()
        {}
        
        // ------------ Abstact Methods ------------
        
        protected abstract void ErrorEntrainement();
        protected abstract int[] GetLayerDimension();
        protected abstract void UseBrain();

        public abstract void SetToTest();
        
        
        // ------------ Brain ------------

        protected void ErrorInput(double[] input)
        {
            // vérifier qu'il n'a pas de problème avec les valeurs de l'input
            int l = input.Length;
            for (int i = 0; i < l; i++)
            {
                if (input[i] < -0.1 || input[i] > 1.1)
                {
                    Debug.Log($"input[{i}] = {input[i]}");
                }
            }
        }

        protected double[] GetResult(NeuralNetwork neuralNetwork, double[] input)
        {
            // feed et enclencher les neurones
            neuralNetwork.Feed(input);
            neuralNetwork.FrontProp();
            
            // retourner le résultat
            return neuralNetwork.GetResult();
        }

        protected double[] InputJump(double minDist, double height)
        {
            return new [] {minDist / MaxDistJump, height / capsule.Height, GetSpeed() / SprintSpeed};
        }
        
        // ------------ Event ------------

        protected override void WhenBlock()
        {
            Entrainement.EndTest();
        }

        private void OnCollisionEnter(Collision other)
        {
            OnCollision(other);
        }

        private void OnCollisionStay(Collision other)
        {
            OnCollision(other);
        }

        protected abstract void OnHighCollision(Collision other);

        private void OnCollision(Collision other)
        {
            foreach (ContactPoint contact in other.contacts)
            {
                if (contact.point.y - Tr.position.y > capsule.Rayon)
                {
                    OnHighCollision(other);
                    return;
                }
            }
        }
    }
}