using System;
using System.Collections.Generic;
using Script.Animation;
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
        
        // ------------ Attributs ------------
        
        // cette liste va contenir la position des chasseurs lorsque le bot les a "vu"
        // si le bot n'en a pas vu, la liste est vide
        private Dictionary<Chasseur, Vector3> Vus = new Dictionary<Chasseur, Vector3>();

        // fuite
        private List<Vector3> planFuite;
        
        /*private float tempsMaxFuite = 3f;
        private float tempsRestantFuite;
        private float distanceFuite;*/
        
        // ------------ Setter ------------

        private void SetEtatPoule()
        {
            etat = Etat.Poule;
            Anim.Set(HumanAnim.Type.Sit);
        }
        
        // ------------ Constructeurs ------------

        protected override void AwakeBot()
        {
            RotationSpeed = 700;
        }

        protected override void StartBot()
        {}

        // ------------ Upadate ------------
        protected override void UpdateBot()
        {
            if (etat == Etat.Fuite)
            {
                Fuir();
            }
            else if (etat == Etat.Attend)
            {
                Tourner();
                
                foreach (PlayerClass chasseur in GetPlayerInMyVision(TypePlayer.Chasseur))
                {
                    // ce sont forcément des chasseurs
                    NewVu((Chasseur)chasseur);
                }
            }
            else if (etat == Etat.FuiteSansPlan)
            {
                // attend son plan, ne fait strictement rien
            }
            else if (etat == Etat.Poule)
            {
                if (GetPlayerInMyVision(TypePlayer.Chasseur).Count == 0)
                {
                    Anim.Stop(HumanAnim.Type.Sit);
                    etat = Etat.Attend;
                    running = Running.Arret;
                }
            }
        }

        // ------------ Méthodes ------------
        private void NewVu(Chasseur vu) // en gros, changement de direction
        {
            // on le rajoute ou update de sa position
            Vus[vu] = vu.transform.position;
            
            // cherche un plan bien rodé vers une destination stratégique
            Vector3 dest;
            dest = BotManager.Instance.GetGoodSpot(this, Vus[vu]);

            if (SimpleMath.IsEncadré(dest, Vector3.zero)) // aucun bon spot
            {
                // n'a pas de destination
                SetEtatPoule();
            }
            else // attend son plan de fuite
            {
                RayGaz.GetPath(Tr.position, dest, RecepRayGaz);
                etat = Etat.FuiteSansPlan;
            }
        }

        private void RecepRayGaz(List<Vector3> path)
        {
            if (path.Count == 0)
            {
                // n'a pas de destination, n'a vraiment pas de plan...
                SetEtatPoule();
            } 
            else
            {
                // part en cavale
                planFuite = path;
                etat = Etat.Fuite;
                running = Running.Course;
            
                /*foreach (Vector3 p in planFuite)
                {
                    TestRayGaz.CreatePointPath(p);
                }*/
            }
        }

        private void Fuir()
        {
            int len = planFuite.Count;

            // s'il a finit une étape de son plan
            if (IsArrivé(planFuite[len - 1], 0.5f))
            {
                planFuite.RemoveAt(len - 1);
                len -= 1;
                
                if (len == 0) // il finit sa cavale,...
                {
                    MoveAmount = Vector3.zero; // ...il s'arrête...
                    etat = Etat.Attend;
                    running = Running.Arret;
                    Vus.Clear();
                    Anim.StopContinue();
                    return; // ...et ne fait rien d'autre
                }
                
                CalculeRotation(planFuite[len - 1]);
            }
            
            GestionRotation(planFuite[len - 1]);
        }

        // ------------ Event ------------
        
        // bloqué
        protected override void WhenBlock()
        {
            AmountRotation = 180;
            etat = Etat.Attend;
            running = Running.Arret;
        }
        
        // collision
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