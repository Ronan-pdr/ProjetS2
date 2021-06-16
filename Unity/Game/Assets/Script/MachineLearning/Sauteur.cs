using System;
using Script.Animation;
using Script.Bot;
using Script.Graph;
using Script.Test;
using UnityEngine;
using Random = System.Random;

namespace Script.MachineLearning
{
    public class Sauteur : BotClass
    {
        // ------------ Attributs ------------

        private NeuralNetwork _neurones;
        private const float MaxDist = 3;
        private const float Decoupage = 10;
        
        // pour l'entrainement aux sauts
        private EntrainementSaut _entrainementSaut;
        
        // ------------ Getter ------------

        public NeuralNetwork Neurones => _neurones;
        
        // ------------ Setter ------------

        public void SetEntrainementSaut(EntrainementSaut value)
        {
            _entrainementSaut = value;
        }

        public void SetNeurone(NeuralNetwork value)
        {
            _neurones = value;
        }

        // ------------ Constructeur ------------

        protected override void AwakeBot()
        {
            // 3 entrées :
            // - la distance entre le bot et l'obstacle
            // - la hauteur du premier obstacle
            // - la vitesse du bot
            
            // 2 sorties :
            // - should jump (possible de passer)
            
            int[] neurones = {3, 1};
            _neurones = new NeuralNetwork(neurones);
        }

        protected override void StartBot()
        {
            SetToRace();
            InvokeRepeating(nameof(PotentielJump), 0.1f, 0.1f);
            //Invoke(nameof(PotentielJump), 0.5f);
        }
        
        // ------------ Update ------------

        protected override void UpdateBot()
        {}

        private void PotentielJump()
        {
            if (!Grounded)
                return;

            Vector3 pos = Tr.position;
            
            float minDist = MaxDist;
            float height = 0;
            float ecart = capsule.Height / Decoupage;
            
            Ray ray = new Ray(pos + Vector3.forward * 0.1f, Vector3.forward);
            
            // trouver la hauteur et la distance du premier obstacle
            for (int i = 1; i < Decoupage; i++)
            {
                ray.origin += Vector3.up * ecart;

                if (Physics.Raycast(ray, out RaycastHit hit, minDist))
                {
                    //Line.Create(ray.origin, hit.point, 250);
                    minDist = hit.distance;
                    height = ray.origin.y - pos.y;
                }
            }

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
            _neurones.Feed(input);
            _neurones.FrontProp();
            
            // récupérer le résultat
            double[] values = _neurones.GetResult();

            if (values[0] > 0.5f)
            {
                Jump();
                _entrainementSaut.Malus();
            }
        }
        
        // ------------ Public Methods ------------
        
        public void SetToRace()
        {
            Anim.Stop(HumanAnim.Type.Sit);
            running = Running.Marche;
        }

        // ------------ Event ------------

        protected override void WhenBlock()
        {
            _entrainementSaut.ResetBot();
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
                    _entrainementSaut.ResetBot();
                    return;
                }
            }
        }
    }
}