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
        
        // ------------ Setter ------------
        public void SetCrossPoint(CrossPoint value)
        {
            PointDestination = value;
        }

        // ------------ Constructeurs ------------
        private void Awake()
        {
            AwakeBot();
        }

        public void Start()
        {
            StartBot(); // tout le monde le fait pour qu'il soit parenter
        
            if (!IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
        }

        // ------------ Update ------------
        protected override void UpdateBot()
        {
            if (etat == Etat.Attend) // s'il est en train d'attendre,...
            {
                if (PointDestination.GetNbNeighboor() > 0)
                {
                    FindNewDestination();
                }
                
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
            }
        }

        // ------------ Méthodes ------------
        public void FindNewDestination()
        {
            int nNeighboor = PointDestination.GetNbNeighboor();

            if (nNeighboor > 0)
            {
                PointDestination = PointDestination.GetNeighboor(Random.Range(0, nNeighboor));
                CalculeRotation(PointDestination.transform.position);
                etat = Etat.EnChemin;
            }
            else
            {
                etat = Etat.Attend;
            }
        }
        
        // ------------ Event ------------
        private void OnCollisionEnter(Collision other)
        {
            OnCollisionAux(other);
        }

        private void OnCollisionExit(Collision other)
        {
            OnCollisionAux(other);
        }

        private void OnCollisionAux(Collision other)
        {
            if (!IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
            
            if (other.gameObject == gameObject) // si c'est son propre corps qu'il a percuté
                return;

            if (etat == Etat.EnChemin) // recalcule seulement quand il avance
            {
                CalculeRotation(PointDestination.transform.position);
            }
        }
    }
}