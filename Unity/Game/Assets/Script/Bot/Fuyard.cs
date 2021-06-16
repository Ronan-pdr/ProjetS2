using System;
using System.Collections.Generic;
using Script.Animation;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Graph;
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
        private enum Etat
        {
            Attend,
            FuiteSansPlan,
            Fuite,
            Poule
        }

        private Etat etat = Etat.Attend;
        
        // ------------ Attributs ------------

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
        {
            BotManager.AddFuyard(this);
        }

        // ------------ Update ------------
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
                    NewVu(chasseur.transform.position);
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
        
        private void NewVu(Vector3 posChasseur)
        {
            // cherche un plan bien rodé vers une destination stratégique
            CrossPoint dest = BotManager.Instance.GetEscapeSpot(this, posChasseur);

            if (dest is null ||
                Calcul.Distance(dest.transform.position, Tr.transform.position) < 5)
            {
                // aucun bon spot
                SetEtatPoule();
            }
            else // attend son plan de fuite
            {
                Vector3 pos = Tr.position;
                
                CrossPoint start = CrossManager.Instance.GetNearestPoint(pos);
                
                GraphPathFinding.GetPath(start, dest, name, RecepPathEscape);
                etat = Etat.FuiteSansPlan;
            }
        }

        private void RecepPathEscape(List<Vector3> path)
        {
            int l = path.Count;
            
            if (l == 0)
            {
                // n'a pas de destination, n'a vraiment pas de plan...
                SetEtatPoule();
            }
            else
            {
                Vector3 pos = Tr.position;
                
                if (l == 1 && Calcul.Distance(pos, path[0]) < 5)
                {
                    SetEtatPoule();
                    return;
                }
                
                // part en cavale
                planFuite = path;
                etat = Etat.Fuite;
                running = Running.Course;

                for (int i = l - 1; i >= 1 && capsule.CanIPass(pos,
                    Calcul.Diff(planFuite[i-1], pos),
                    Calcul.Distance(planFuite[i-1], pos)); i--)
                {
                    planFuite.RemoveAt(i);
                }
                
                foreach (Vector3 p in planFuite)
                {
                    TestRayGaz.CreateMarqueur(p, TestRayGaz.Couleur.Brown);
                }
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
                    MoveAmount = Vector3.zero; // ...il s'arrête,...
                    etat = Etat.Attend;
                    running = Running.Arret;
                    AmountRotation = 180; // ...se retourne...
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