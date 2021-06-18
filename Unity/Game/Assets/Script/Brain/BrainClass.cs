using System;
using System.IO;
using Script.EntityPlayer;
using Script.Graph;
using Script.MachineLearning;
using Script.Manager;
using UnityEngine;
using Random = System.Random;

namespace Script.Brain
{
    public abstract class BrainClass
    {
        // ------------ Attributs ------------

        protected HumanCapsule Capsule;
        protected NeuralNetwork Neurones;
        private Random _rnd;
        
        // ------------ Constructeur ------------

        private void SetAttributs()
        {
            Capsule = MasterManager.Instance.GetHumanCapsule();
            _rnd = new Random();
        }

        protected void Set()
        {
            NewNeuralNetwork();
            SetAttributs();
        }
        
        protected void Set(int numero)
        {
            string path = GetPath(numero);

            if (File.Exists(path))
            {
                Neurones = NeuralNetwork.Restore(path);
            }
            else
            {
                Debug.Log($"Le numéro {numero} du {GetNameDirectory()} du saut n'existe pas");
                NewNeuralNetwork();
            }

            SetAttributs();
        }

        protected abstract void NewNeuralNetwork();


        // ------------ Public Methods ------------
        
        public void UpdateNeurones(BrainClass brain1, BrainClass brain2)
        {
            switch (_rnd.Next(2))
            {
                case 0:
                    // le muter
                    Neurones = new NeuralNetwork(brain1.Neurones, true);
                    break;
                default:
                    // enfanter
                    Neurones = new NeuralNetwork(brain1.Neurones, false);
                    Neurones.Crossover(brain2.Neurones);
                    break;
            }
        }

        public void Save(int numero)
        {
            Neurones.Save(GetPath(numero));
        }
        
        public static (double dist, double height) GetStaticDistHeightFirstObstacle(Transform tr, double distMax)
        {
            return GetStaticDistHeightFirstObstacle(tr, distMax, MasterManager.Instance.GetHumanCapsule());
        }
        
        public static (double dist, double height) GetStaticDistHeightFirstObstacle(Transform tr, double distMax, HumanCapsule capsule)
        {
            float decoupage = 10;
            float minDist = (float)distMax;
            float height = 0;
            float ecart = capsule.Height / decoupage;
            Vector3 pos = tr.position;
            
            Ray ray = new Ray(pos + Vector3.forward * 0.1f, tr.TransformDirection(Vector3.forward));
            
            // trouver la hauteur et la distance du premier obstacle
            for (int i = 2; i < decoupage; i++)
            {
                ray.origin += Vector3.up * ecart;

                if (Physics.Raycast(ray, out RaycastHit hit, minDist))
                {
                    //Line.Create(ray.origin, hit.point, 250);
                    minDist = hit.distance;
                    height = ray.origin.y - pos.y;
                }
            }

            return (minDist, height);
        }

        // ------------ Protected Methods ------------

        protected string GetPath(int numero) => $"Build/{GetNameDirectory()}/{numero}";

        protected abstract string GetNameDirectory();
        
        // detection
        protected (double dist, double height) GetDistHeightFirstObstacle(Transform tr, double distMax)
        {
            return GetStaticDistHeightFirstObstacle(tr, distMax, Capsule);
        }

        protected double[] GetResult(NeuralNetwork neuralNetwork, double[] input)
        {
            // feed et enclencher les neurones
            neuralNetwork.Feed(input);
            neuralNetwork.FrontProp();
            
            // retourner le résultat
            return neuralNetwork.GetResult();
        }
        
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
    }
}