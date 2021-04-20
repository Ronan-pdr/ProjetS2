using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.Test;
using Script.TeteChercheuse;
using Script.Tools;
using UnityEngine;

namespace Script.Bot
{
    public class Fuyard : BotClass
    {
        // ------------ Etat ------------
        protected enum Etat
        {
            Attend,
            FuiteSansPlan,
            Fuite,
            Poule
        }

        private Etat etat = Etat.Attend;
        
        // cette liste va contenir la position des chasseurs lorsque le bot les a "vu"
        // si le bot n'en a pas vu, la liste est vide
        private List<(Chasseur chasseur, Vector3 position)> Vus = new List<(Chasseur chasseur, Vector3 position)>();

        // fuite
        private List<Vector3> planFuite;
        
        /*private float tempsMaxFuite = 3f;
        private float tempsRestantFuite;
        private float distanceFuite;*/
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            AwakeBot();
        }

        private void Start()
        {
            rotationSpeed = 600;
            
            StartBot(); // tout le monde le fait pour qu'il soit parenter
            
            master = MasterManager.Instance;
        }

        // ------------ Méthodes ------------
        private void Update()
        {
            if (!IsMyBot())
                return;
            
            // quoi que soit son état, il fait ça
            UpdateBot();

            if (etat == Etat.Fuite)
            {
                Fuir();
            }
            else if (etat == Etat.Attend)
            {
                foreach (PlayerClass chasseur in GetPlayerInMyVision(TypePlayer.Chasseur))
                {
                    // ce sont forcément des chasseurs
                    NewVu((Chasseur)chasseur);
                }
            }
        }

        private void FixedUpdate()
        {
            FixedUpdateBot();
        }

        private void NewVu(Chasseur vu) // en gros, changement de direction
        {
            int i;
            int len = Vus.Count;
            for (i = 0; i < len && Vus[i].chasseur != vu; i++)
            {}

            if (i < len) // le chasseur vu était déjà dans les 'Vus'
                Vus[i] = (vu, vu.transform.position); // update de sa position
            else
                Vus.Add((vu, vu.transform.position)); // on le rajoute
            
            // cherche un plan bien rodé vers une destination stratégique
            Vector3 dest;
            dest = BotManager.Instance.GetGoodSpot(this, Vus[0].position);

            if (SimpleMath.IsEncadré(dest, Vector3.zero)) // aucun bon spot
            {
                // n'a pas de destination
                etat = Etat.Poule;
                ActiverAnimation("Assis");
            }
            else // attend son plan de fuite
            {
                RayGaz.GetPath(Tr.position, dest, RecepRayGaz);
                etat = Etat.FuiteSansPlan;
            }
        }

        public void RecepRayGaz(List<Vector3> path)
        {
            if (path.Count == 0)
            {
                // n'a pas de destination, n'a vraiment pas de plan...
                etat = Etat.Poule;
                ActiverAnimation("Assis");
            } 
            else
            {
                // part en cavale
                planFuite = path;
                etat = Etat.Fuite;
            
                /*foreach (Vector3 p in planFuite)
                {
                    TestRayGaz.CreatePointPath(p);
                }*/
            }
        }

        /*private void OldFuite()
        {
            // pour l'instant je vais juste gérer le cas où y'a qu'un chasseur
            var position = Tr.position;
            float angleY = Calcul.Angle(0, position, Vus[0].position, Calcul.Coord.Y);
            
            angleY += 180 * (angleY > 0 ? -1 : 1); // il va rotater pour aller le plus loin possible des chasseur

            // teste ses directions pour déterminer s'il n'y a pas d'obstacle
            int ecartAngle = 0; // prendra les valeurs successives 0 ; 1 ; -1 ; 2 ; -2 ; 3 ; -3...
            
            for (int j = 0; !RayGaz.CanIPass(capsule, Tr.position, Calcul.FromAngleToDirection(angleY + ecartAngle), distanceFuite) && ecartAngle < 130; j++)
            {
                ecartAngle += j * 5 * (j % 2 == 1 ? 1 : -1);
            }

            AmountRotation = Calcul.GiveAmoutRotation(angleY + ecartAngle, Tr.eulerAngles.y);
            
            tempsRestantFuite = tempsMaxFuite; // il regonfle son temps de fuite son temps de fuite
            etat = Etat.Fuite;
        }*/

        protected override void FiniDeTourner()
        {} // lorsqu'il a fini de tourner, il ne fait rien de plus

        private void Fuir()
        {
            int len = planFuite.Count;

            float dist = Calcul.Distance(Tr.position, planFuite[len - 1], Calcul.Coord.Y);

            if (dist < 2)
            {
                // marche parce que proche
                SetMoveAmount(Vector3.forward, WalkSpeed);
                ActiverAnimation("Avant");
            }
            else
            {
                // court
                SetMoveAmount(Vector3.forward, SprintSpeed);
                ActiverAnimation("Course");
            }

            // s'il a finit une étape de son plan
            if (dist < 0.5f)
            {
                planFuite.RemoveAt(len - 1);
                
                if (len == 1) // il finit sa cavale,...
                {
                    MoveAmount = Vector3.zero; // ...il s'arrête...
                    etat = Etat.Attend;
                    Vus.Clear();
                    AnimationStop();
                    return; // ...et ne fait rien d'autre
                }
                
                CalculeRotation(planFuite[len - 2]);
            }
            
            // il recalcule sa rotation tous les 0.5f
            if (Time.time - LastCalculRotation > 0.5f)
            {
                CalculeRotation(planFuite[len - 1]);
            }
            
            if (SimpleMath.Abs(AmountRotation) > 0)
                Tourner();
        }
        
        // Event
        private void OnTriggerEnter(Collider other)
        {
            OnCollisionAux(other);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            OnCollisionAux(other.collider);
        }

        private void OnCollisionAux(Collider other)
        {
            if (!IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
        
            if (other.gameObject == gameObject) // si c'est son propre corps qu'il a percuté
                return;

            if (other.gameObject.GetComponent<BalleFusil>())
            {
                Jump();
            }
        }
    }
}