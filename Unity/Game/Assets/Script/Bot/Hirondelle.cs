using System;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Test;
using Script.Tools;
using UnityEngine;
using UnityEngine.WSA;

namespace Script.Bot
{
    public class Hirondelle : BotClass
    {
        // ------------ SerializeField ------------

        [Header("Entourage")]
        [SerializeField] private Entourage obstacles;
        [SerializeField] private Entourage voisins;
        
        // ------------ Attributs ------------

        private float _nextRegard;

        private bool _synchro;
        
        // ------------ Setter ------------

        private void SetSynchro(bool value)
        {
            _synchro = value;
            voisins.gameObject.SetActive(value);
        }

        private int Mod(float a, int b) => SimpleMath.Mod((int) a, b);

        // ------------ Constructeur ------------
        protected override void AwakeBot()
        {
            RotationSpeed = 500;
            running = Running.Marche;
            TranquilleVitesse = 2;
            
            SetSynchro(true);
            
            // l'entourage
            
            obstacles.Set(o => o.CompareTag("Respawn"));
            voisins.Set(g => g.GetComponent<Hirondelle>());
        }

        protected override void StartBot()
        {
            AmountRotation = UnityEngine.Random.Range(-180, 180);
            //Syncronisation.Instance.AddHirondelle(this);

            InvokeRepeating(nameof(GererObstacles), 1, 0.4f);
            InvokeRepeating(nameof(EcartUpdate), 1, 0.1f);
        }
    
        // ------------ Update ------------
    
        protected override void UpdateBot()
        {
            Tourner();

            if (Input.GetKeyDown(KeyCode.P))
            {
                _synchro = !_synchro;

                if (_synchro)
                {
                    Debug.Log("Synchro");
                }
                else
                {
                    Debug.Log("Desynchro");
                }
            }
        }
        
        private void EcartUpdate()
        {
            if (_synchro)
            {
                Synchronisation();
            }
            else
            {
                // tourner légerement
                AmountRotation += UnityEngine.Random.Range(-10f, 10f);
            }
        }
    
        // ------------ Méthodes ------------

        private void GererObstacles()
        {
            if (Time.time < _nextRegard)
                return;

            _nextRegard = Time.time + 0.4f;
            
            List<Vector3> cage = obstacles.GetList();
            int l = cage.Count;

            if (l == 1)
            {
                SingleObstacle(cage[0]);
            }
            else if (l > 2)
            {
                AmountRotation += Autour(180);
            }
        }

        private void TwoObstacle(Vector3 obstacle1, Vector3 obstacle2)
        {
            float angle1 = GetAngle(obstacle1);
            float angle2 = GetAngle(obstacle2);

            int moy = (int) ((angle1 + angle2) / 2);

            if (SimpleMath.Abs(angle1 - angle2) >= 180)
            {
                Debug.Log("vers la moyenne");
                // aller vers la moyenne
                AmountRotation += Autour(moy);
            }
            else
            {
                
                // aller à l'inverse de la moyenne
                float angle = Calcul.GiveAmoutRotation(moy + 180, Tr.eulerAngles.y);
                Debug.Log($"l'inverse de la moyenne, angle = '{angle}'");
                AmountRotation += angle;
            }
        }

        private void SingleObstacle(Vector3 obstacle)
        {
            float angle = GetAngle(obstacle);
            
            if (SimpleMath.Abs(angle) > 110)
            {
                // on s'ent fout des obstacles dans ton dos ou presque
                return;
            }
            
            if (angle > 50)
            {
                AmountRotation -= PetitVirage();
            }
            else if (angle >= 0)
            {
                AmountRotation -= GrosVirage();
            }
            else if (angle > -50)
            {
                AmountRotation += GrosVirage();
            }
            else
            {
                AmountRotation += PetitVirage();
            }
        }

        private float GetAngle(Vector3 pos)
        {
            return Calcul.Angle(Tr.eulerAngles.y, Tr.position, pos, Calcul.Coord.Y);
        }
        
        // ------------ Synchronisation ------------

        private void Synchronisation()
        {
            foreach (KeyValuePair<GameObject, Vector3> e in voisins.GetDict())
            {
                float dist = Calcul.Distance(Tr.position, e.Value);
                
                if (dist < 1.5)
                {
                    // trop proche
                    Eloigner(e.Value);
                }
                else if (dist < 2)
                {
                    // parfait
                    Aligner(e.Key.GetComponent<Hirondelle>());
                }
                else
                {
                    // trop loin
                    Rapprocher(e.Value);
                }
            }
        }

        private void Eloigner(Vector3 pos)
        {
            float angle = GetAngle(pos);
            float abs = SimpleMath.Abs(angle);
            
            if (10 <= abs && abs <= 170)
            {
                // s'éloigner
                AmountRotation += 4 * (angle > 0 ? -1 : 1);
            }
        }

        private void Aligner(Hirondelle hirondelle)
        {
            float angle = hirondelle.Tr.eulerAngles.y - Tr.eulerAngles.y;
            float abs = SimpleMath.Abs(angle);

            if (5 <= abs && abs <= 180)
            {
                // s'aligner
                AmountRotation += 2 * (angle > 0 ? 1 : -1);
            }
        }
        
        private void Rapprocher(Vector3 pos)
        {
            float angle = GetAngle(pos);
            float abs = SimpleMath.Abs(angle);
            
            if (5 <= abs && abs <= 180)
            {
                // s'éloigner
                AmountRotation += 3 * (angle > 0 ? 1 : -1);
            }
        }
        
        // ------------ Random ------------

        private int PetitVirage()
        {
            return Virage(50, 90);
        }
        
        private int GrosVirage()
        {
            return Virage(100, 140);
        }

        private int Virage(int begin, int end)
        {
            return UnityEngine.Random.Range(begin, end);
        }

        private int Autour(int angle)
        {
            return Virage(angle - 20, angle + 20);
        }


        // ------------ Event ------------
        
        protected override void WhenBlock()
        {
            AmountRotation += Autour(180);
            Debug.Log("Je suis bloqué chef");
        }
    }
}
