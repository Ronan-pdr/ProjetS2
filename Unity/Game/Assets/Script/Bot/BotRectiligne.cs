using System.Collections.Generic;
using Script.Tools;
using UnityEngine;
using Random = UnityEngine.Random;
using Script.DossierPoint;
using Script.TeteChercheuse;

namespace Script.Bot
{
    public class BotRectiligne : BotClass
    {
        // ------------ Etat ------------
        public enum Etat
        {
            EnChemin,
            Attend // il attend seulement lorsqu'il est sur un point qui possède 0 voisin
        }
        
        private Etat etat = Etat.Attend;

        private CrossPoint PointDestination;

        private (float time, Vector3 position) block;
        private CrossPoint previousPoint;
        
        // ------------ Setter ------------
        public void SetCrossPoint(CrossPoint value)
        {
            PointDestination = value;
        }

        // ------------ Constructeurs ------------
        protected override void AwakeBot()
        {}

        protected override void StartBot()
        {
            FindNewDestination();
        }

        // ------------ Update ------------
        protected override void UpdateBot()
        {
            if (etat == Etat.Attend) // s'il est en train d'attendre,...
            {
                MoveAmount = Vector3.zero; // ...il ne se déplace pas...
                return; // ...et ne fait rien d'autre
            }
            
            GestionRotation(PointDestination.transform.position);

            if (etat == Etat.EnChemin)
            {
                if (IsArrivé(PointDestination.transform.position)) // arrivé
                {
                    FindNewDestination();
                    AnimationStop();
                }

                ManageBlock();
            }
        }

        // ------------ Méthodes ------------
        public void FindNewDestination()
        {
            int nNeighboor = PointDestination.GetNbNeighboor();

            if (nNeighboor > 0)
            {
                // sauvegarde de sa précédente destination
                previousPoint = PointDestination;
                
                // il repart
                PointDestination = PointDestination.GetNeighboor(Random.Range(0, nNeighboor));
                CalculeRotation(PointDestination.transform.position);
                etat = Etat.EnChemin;
                running = Running.Marche;
                SetMoveAmount(Vector3.forward, TranquilleVitesse);
            }
            else
            {
                etat = Etat.Attend;
                running = Running.Arret;
            }
        }

        private void ManageBlock()
        {
            // vérifier qu'il n'est pas bloqué
            if (SimpleMath.IsEncadré(block.position, Tr.position))
            {
                // s'il semble bloquer à une position
                if (Time.time - block.time > 2)
                {
                    // et que ça fait longtemps
                    PointDestination = previousPoint;
                    // il reva à sa précédente position
                }
            }
            else
            {
                // set block
                block.time = Time.time;
                block.position = Tr.position;
            }
        }
    }
}