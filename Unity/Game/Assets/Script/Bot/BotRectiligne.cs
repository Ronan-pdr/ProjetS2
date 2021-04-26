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
        
        // pour quand il est bloqué
        private CrossPoint previousPoint;

        // ------------ Setter ------------
        public void SetCrossPoint(CrossPoint value)
        {
            PointDestination = value;
            FindNewDestination();
        }

        // ------------ Constructeurs ------------
        protected override void AwakeBot()
        {
            RotationSpeed = 600;
        }

        protected override void StartBot()
        {}

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
                if (IsArrivé(PointDestination.transform.position, 0.3f)) // arrivé
                {
                    FindNewDestination();
                    AnimationStop();
                }
            }
        }
        
        // Bloqué
        protected override void WhenBlock()
        {
            PointDestination = previousPoint;
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
    }
}