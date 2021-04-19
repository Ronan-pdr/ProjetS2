using System;
using System.Collections.Generic;
using Script.Bot;
using Script.EntityPlayer;
using Script.Test;
using Script.Tools;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class Guide : BotClass
    {
        // ------------ Etat ------------
        private enum Etat
        {
            Attend,
            Guidage,
            Arrivé
        }

        private Etat etat;
        
        // ------------ Attributs ------------
        
        private float timeLastRegard;
        private List<Vector3> path;
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            AwakeBot();
        }

        public void Start()
        {
            rotationSpeed = 600;
            etat = Etat.Attend;
            
            // tout le monde le fait pour qu'il soit parenter
            StartBot();
        }
        
        // ------------ Méthodes ------------
        private void Update()
        {
            if (!IsMyBot())
                return;
            
            switch (etat)
            {
                case Etat.Attend:
                    if (IsDepart())
                    {
                        timeLastRegard = Time.time;
                        Depart();
                    }
                    break;
                case Etat.Guidage:
                    Guidage();
                    break;
                default:
                    throw new Exception($"Le cas de {etat} n'est pas encore géré");
            }
        }
        
        private void FixedUpdate()
        {
            FixedUpdateBot();
        }
        
        private bool IsDepart()
        {
            return Time.time - timeLastRegard > 1 &&
                   IsPlayerInMyVision(TypePlayer.Player).Count > 0;
        }

        private void Depart()
        {
            path = LabyrintheManager.Instance.GetBestPath(Tr.position);

            if (path.Count > 0)
            {
                etat = Etat.Guidage;
            }
        }

        private void Guidage()
        {
            MoveAmount = Vector3.forward * SprintSpeed;
            
            // s'il a finit une étape de son plan
            if (IsArrivé(path[0]))
            {
                path.RemoveAt(0);
                
                // Est - il arrivé ?
                if (path.Count == 0)
                {
                    // Il ne fait plus jamais rien
                    enabled = false;
                    return;
                }
                
                AmountRotation = Calcul.Angle(Tr.eulerAngles.y, Tr.position, path[0], Calcul.Coord.Y);
            }
            
            // il recalcule sa rotation tous les 0.5f
            if (Time.time - LastCalculRotation > 0.5f)
            {
                CalculeRotation(path[0]);
            }

            if (SimpleMath.Abs(AmountRotation) > 0)
                Tourner();
        }

        protected override void FiniDeTourner()
        {} // lorsqu'il a fini de tourner, il ne fait rien de plus

        protected override void Die()
        {
            throw new System.NotImplementedException();
        }
    }
}