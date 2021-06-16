using System;
using Script.Bot;
using UnityEngine;

namespace Script.MachineLearning
{
    public class Autonome : BotClass
    {
        // ------------ SerializeField ------------

        [SerializeField] private bool memory;
        
        // ------------ Attributs ------------

        private NeuralNetwork _neurones;
        private const float MaxDist = 3;
        private const float Decoupage = 10;
        
        // pour l'entraiment aux sauts
        private EntrainementSaut _entrainementSaut;
        
        // ------------ Getter ------------

        public NeuralNetwork Neurones => _neurones;
        
        // ------------ Setter ------------

        public void SetEntrainemetSaut(EntrainementSaut value)
        {
            _entrainementSaut = value;
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
            // - impossible de passer
            
            int[] neurones = {3, 2};
            _neurones = new NeuralNetwork(neurones);
        }

        protected override void StartBot()
        {
            running = Running.Marche;
            transform.parent = _entrainementSaut.gameObject.transform;
            InvokeRepeating(nameof(Jumping), 0.1f, 0.1f);
        }
        
        // ------------ Update ------------

        protected override void UpdateBot()
        {}

        private void Jumping()
        {
            Vector3 pos = Tr.position;
            
            float minDist = MaxDist;
            float height = 0;
            float ecart = capsule.Height / Decoupage;
            
            Ray ray = new Ray(pos, Vector3.forward);
            
            // trouver la hauteur et la distance du premier obstacle
            for (int i = 0; i < 10; i++)
            {
                ray.origin += Vector3.up * ecart;

                if (Physics.Raycast(ray, out RaycastHit hit, minDist))
                {
                    minDist = hit.distance;
                    height = ray.origin.y - pos.y;
                }
            }

            double[] input =
            {
                minDist / MaxDist, height / capsule.Height, WalkSpeed
            };
            
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
        
        // ------------ Private Methods ------------

        // ------------ Event ------------

        protected override void WhenBlock()
        {}

        private void OnCollisionEnter()
        {
            _entrainementSaut.ResetBot();
        }
    }
}