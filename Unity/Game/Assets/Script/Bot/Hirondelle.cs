using System;
using Script.EntityPlayer;
using Script.Test;
using Script.Tools;
using UnityEngine;

namespace Script.Bot
{
    public class Hirondelle : BotClass
    {
        // ------------ Attributs ------------
    
        private const int Vision = 3;

        private float _nextRegard;

        private bool _synchro;
        
        // ------------ Setter ------------

        private void SetSynchro(bool value)
        {
            _synchro = value;
        }

        public void Eloigner(Vector3 pos)
        {
            Vector3 diff = Calcul.Diff(pos, Tr.position);

            Aligner(Calcul.BetterArctan(diff.x, diff.z) + 180);
        }

        public void Aligner(float value, float ecartement = 1f)
        {
            float rotY = Mod(value, 360);
            float y = Mod(Tr.eulerAngles.y, 360);
            int coef;

            float diff = SimpleMath.Abs(rotY - y);

            if (diff < 10)
            {
                coef = 0;
            }
            else if (rotY > y)
            {
                coef = rotY - y < 180 ? 1 : -1;
            }
            else
            {
                coef = y - rotY < 180 ? -1 : 1;
            }

            AmountRotation += coef * ecartement;
        }
        
        public void Rapprocher(Vector3 pos)
        {
            Vector3 diff = Calcul.Diff(pos, Tr.position);

            Aligner(Calcul.BetterArctan(diff.x, diff.z));
        }

        private int Mod(float a, int b) => SimpleMath.Mod((int) a, b);

        // ------------ Constructeur ------------
        protected override void AwakeBot()
        {
            RotationSpeed = 100;
            running = Running.Marche;
            TranquilleVitesse = 1;
            
            SetSynchro(true);
        }

        protected override void StartBot()
        {
            AmountRotation = UnityEngine.Random.Range(-180, 180);
            Syncronisation.Instance.AddHirondelle(this);
        }
    
        // ------------ Update ------------
    
        protected override void UpdateBot()
        {
            Tourner();

            GererObstacle();

            if (_synchro)
            {
                
            }
            else
            {
                // tourner légerement
                AmountRotation += UnityEngine.Random.Range(-5f, 5f);
            }
        }
    
        // ------------ Méthodes ------------

        private void GererObstacle()
        {
            if (Time.time < _nextRegard)
            {
                return;
            }

            _nextRegard = Time.time + 0.5f;

            Ray ray = new Ray(Tr.position + 0.5f * Vector3.up, Tr.TransformDirection(Vector3.forward));

            // obstacle droit devant
            if (Obstacle(ray, out RaycastHit h, Vision))
            {
                //TestRayGaz.CreateMarqueur(h.point, TestRayGaz.Couleur.Red);
                
                AmountRotation = WhenObstacleInFront(ray);
            }
            else
            {
                ray.direction = Tr.TransformDirection(Vector3.right);
                if (Obstacle(ray, Vision / 2))
                {
                    // obstacle à droite
                    AmountRotation = -30;
                }
                else
                {
                    ray.direction = Tr.TransformDirection(Vector3.left);
                    
                    if (Obstacle(ray, Vision / 2))
                    {
                        // obstacle à droite
                        AmountRotation = 30;
                    }
                }
            }
        }

        private int WhenObstacleInFront(Ray ray)
        {
            ray.direction = Tr.TransformDirection(new Vector3(1, 0, 0));
            
            if (!Obstacle(ray, out RaycastHit hit1, Vision * 2))
            {
                Debug.Log(1);
                // Pas d'obstacle à droite --> bar à tribord toute
                return Virage();
            }
            
            ray.direction = Tr.TransformDirection(new Vector3(-1, 0, 0));

            if (!Obstacle(ray, out RaycastHit hit2, Vision * 2))
            {
                Debug.Log(2);
                // Pas d'obstacle à gauche --> bar à vabord toute
                return -Virage();
            }

            if (hit1.distance > hit2.distance)
            {
                Debug.Log(3);
                // Plus de place à droite --> bar à tribord toute
                return Virage();
            }
            
            Debug.Log(4);
            // Plus de place à gauche --> bar à vabord toute
            return -Virage();
        }
        
        private bool Obstacle(Ray r, out RaycastHit hit, int max)
        {
            return Physics.Raycast(r, out hit, max) && !hit.collider.GetComponent<Entity>();
        }
        
        private bool Obstacle(Ray r, int max)
        {
            return Obstacle(r, out RaycastHit h, max);
        }

        private int Virage()
        {
            return UnityEngine.Random.Range(70, 110);
        }

    
        // ------------ Event ------------
        protected override void WhenBlock()
        {
            Debug.Log("On est bloqué chef !");
        }
    }
}
