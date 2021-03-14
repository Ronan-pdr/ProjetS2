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
            Attend // il attend seulement lorsqu'il est sur un point qui possède encore 0 voisin
        }
        private Etat etat = Etat.Attend;

        //Destination
        private CrossPoint PointDestination;
    
        //Rotation
        private float rotationSpeed = 200;
        private float amountRotation;
    
        //Ecart maximum entre le point et sa position pour qu'il soit considéré comme arrivé à destination
        private float ecartDistance = 0.5f;
    
        //Le bot va recalculer automatiquement sa trajectoire au bout de 'ecartTime'
        private float ecartTime = 1;
        private float lastCalculRotation; //cette variable contient le dernier moment durant lequel le bot à recalculer sa trajectoire

        //Getter
    
        public Etat GetEtat() => etat;
        
        // Setter

        public void SetPointDestination(CrossPoint value)
        {
            PointDestination = value;
        }

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

            if (Time.time - lastCalculRotation > ecartTime) // il recalcule sa rotation tous les 'ecartTime'
            {
                FindAmountRotation();
            }
        
            if (etat == Etat.EnChemin)
            {
                if (Calcul.Distance(Tr.position, PointDestination.transform.position) < ecartDistance) // arrivé
                {
                    FindNewDestination();
                    anim.enabled = false;
                }
                else // avancer
                    Avancer();
            }
            else // se troune
            {
                MoveAmount = Vector3.zero; // Le bot rectiligne n'avançera jamais lorqu'il tournera
                Tourner();
            }
        }

        private void FixedUpdate()
        {
            if (!IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
        
            MoveEntity();
        }

        public void FindNewDestination()
        {
            int nNeighboor = PointDestination.GetNbNeighboor();

            if (nNeighboor > 0)
            {
                PointDestination = PointDestination.GetNeighboor(Random.Range(0, nNeighboor));
                FindAmountRotation(); // va aussi instancier 'etat'
            }
            else
            {
                etat = Etat.Attend;
            }
        }

        // Cette fonction trouve le degré nécessaire (entre ]-180, 180]) afin que le soit orienté face à sa destination
        public void FindAmountRotation() // Change aussi l'état du joueur
        {
            amountRotation = Calcul.Angle(Tr.eulerAngles.y, Tr.position, PointDestination.transform.position);

            if (SimpleMath.Abs(amountRotation) < 5) // Si le dégré est négligeable, le bot continue sa course
            {
                etat = Etat.EnChemin; // va directement avancer
            }
            else
            {
                etat = Etat.SeTourne; // va tourner
            }

            lastCalculRotation = Time.time;
        }

        private void Avancer()
        {
            SetMoveAmount(new Vector3(0, 0, 1), SprintSpeed);
        
            anim.enabled = true;
            anim.Play("Avant");
        }

        private void Tourner()
        {
            int sensRotation;
            if (amountRotation >= 0)
                sensRotation = 1;
            else
                sensRotation = -1;

            float yRot = sensRotation * rotationSpeed * Time.deltaTime;

            if (SimpleMath.Abs(amountRotation) < SimpleMath.Abs(yRot)) // Le cas où on a finis de tourner
            {
                Tr.Rotate(new Vector3(0, amountRotation, 0));
                etat = Etat.EnChemin; // il va avancer maintenant
            }
            else
            {
                Tr.Rotate(new Vector3(0, yRot, 0));
                amountRotation -= yRot;
            }
        }
    }
}