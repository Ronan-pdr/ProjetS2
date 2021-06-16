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
        
        protected override void AwakeBot()
        {
            Neurones = new NeuralNetwork(GetLayerDimension());
        }
        
        protected override void StartBot()
        {
            if (!(Entrainement is EntrainementSaut))
            {
                throw new Exception();
            }
            
            SetToTest();
            InvokeRepeating(nameof(UseBrain), 0.1f, 0.1f);
            //Invoke(nameof(PotentielJump), 0.5f);
        }
        
        // ------------ Public Methods ------------
        
        public void SetToTest()
        {
            Anim.Stop(HumanAnim.Type.Sit);
            running = Running.Marche;
        }
        
        // ------------ Abstact Methods ------------

        protected abstract void UseBrain();
        protected abstract void ErrorEntrainement();
        protected abstract int[] GetLayerDimension();
        
        // ------------ Event ------------

        protected override void WhenBlock()
        {
            Entrainement.EndTest();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cage"))
            {
                running = Running.Course;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            OnCollision(other);
        }

        private void OnCollisionStay(Collision other)
        {
            OnCollision(other);
        }

        private void OnCollision(Collision other)
        {
            foreach (ContactPoint contact in other.contacts)
            {
                if (contact.point.y - Tr.position.y > capsule.Rayon)
                {
                    Entrainement.EndTest();
                    return;
                }
            }
        }
    }
}