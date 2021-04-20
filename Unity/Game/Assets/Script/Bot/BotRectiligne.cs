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
        //Etat
        public enum Etat
        {
            EnChemin,
            SeTourne,
            Attend // il attend seulement lorsqu'il est sur un point qui possède 0 voisin
        }
        private Etat etat = Etat.Attend;

        //Destination
        private CrossPoint PointDestination;
        
        //Getter
    
        public Etat GetEtat() => etat;
        
        // Setter
        public void SetCrossPoint(CrossPoint crossPoint)
        {
            PointDestination = crossPoint;
        }

        // constructeurs
        private void Awake()
        {
            AwakeBot();
        }

        public void Start()
        {
            rotationSpeed = 200;
            
            StartBot(); // tout le monde le fait pour qu'il soit parenter
        
            if (!IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
        }

        void Update()
        {
            if (!IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
        
            UpdateBot(); // quoi que soit son état, il fait ça

            if (etat == Etat.Attend) // s'il est en train d'attendre,...
            {
                if (PointDestination.GetNbNeighboor() > 0)
                {
                    FindNewDestination();
                }
                
                MoveAmount = Vector3.zero; // ...il ne se déplace pas...
                return; // ...et ne fait rien d'autre
            }
            
            if (Time.time - LastCalculRotation > 0.5f) // il recalcule sa rotation tous les 'ecartTime'
            {
                FindAmountRotation();
            }

            if (etat == Etat.EnChemin)
            {
                if (IsArrivé(PointDestination.transform.position)) // arrivé
                {
                    FindNewDestination();
                    AnimationStop();
                }
                else // avancer
                    Avancer();
            }
            else // se tourne (etat = Etat.SeTourne)
            {
                MoveAmount = Vector3.zero; // Le bot rectiligne n'avançera jamais lorqu'il tournera
                Tourner();
            }
        }

        private void FixedUpdate()
        {
            FixedUpdateBot();
        }

        public void FindNewDestination()
        {
            int nNeighboor = PointDestination.GetNbNeighboor();

            if (nNeighboor > 0)
            {
                PointDestination = PointDestination.GetNeighboor(Random.Range(0, nNeighboor));
                FindAmountRotation(); // va aussi donner une valeur à 'etat'
            }
            else
            {
                etat = Etat.Attend;
            }
        }

        // Cette fonction trouve le degré nécessaire (entre ]-180, 180]) afin que le soit orienté face à sa destination
        public void FindAmountRotation()
        {
            CalculeRotation(PointDestination.transform.position);
            
            if (SimpleMath.Abs(AmountRotation) < 5) // Si le dégré est négligeable, le bot continue sa course
            {
                etat = Etat.EnChemin; // va directement avancer
            }
            else
            {
                etat = Etat.SeTourne; // va tourner
            }
        }

        private void Avancer()
        {
            SetMoveAmount(new Vector3(0, 0, 1), WalkSpeed);
        
            ActiverAnimation("Avant");
        }

        protected override void FiniDeTourner()
        {
            etat = Etat.EnChemin; // il va avancer maintenant
        }
        
        // Event
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

            if (GetEtat() == Etat.EnChemin) // recalcule seulement quand il avance
            {
                FindAmountRotation();
            }
        }
    }
}