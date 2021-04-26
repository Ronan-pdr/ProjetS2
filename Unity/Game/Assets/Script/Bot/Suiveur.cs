using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.Bot
{
    public class Suiveur : BotClass
    {
        // ------------ Etat ------------
        private enum Etat
        {
            Attend,
            Poursuite,
            Fuite
        }

        private Etat etat;
        
        // ------------ Attributs ------------
        
        private (Chasseur chasseur, Vector3 position) Vu;

        // temps
        private float timeLastRegard;
        private float timeLastFindDest;

        private static float rayonPerimetre = 25;
        
        // destination
        private Vector3 destination;


        // ------------ Constructeurs ------------
        
        protected override void AwakeBot()
        {}

        protected override void StartBot()
        {
            etat = Etat.Attend;
        }
    
        // ------------ Update ------------
        protected override void UpdateBot()
        {
            if (etat == Etat.Attend)
            {
                if (Time.time - timeLastRegard > 0.3f)
                {
                    SearchChasseurWithVision();
                }
            }
            else
            {
                if (etat == Etat.Poursuite)
                {
                    Follow();
                }
                else if (etat == Etat.Fuite)
                {
                    Escape();
                }
                
                MustEscapeFollowOrWait();
                
                GestionRotation(destination, 0.2f);
            }

            Tourner();
        }

        // ------------ Méthodes ------------
        private void SearchChasseurWithVision()
        {
            List<PlayerClass> vus = GetPlayerInMyVision(TypePlayer.Chasseur);
            timeLastRegard = Time.time;
            
            if (vus.Count > 0)
            {
                // Il a désormais un chasseur dans sa vision
                PlayerClass player = vus[0];
                Vu = ((Chasseur) player, player.transform.position);

                MustEscapeFollowOrWait();
            }
        }

        private void MustEscapeFollowOrWait()
        {
            float dist = Calcul.Distance(Tr.position, Vu.position, Calcul.Coord.Y);
            
            if (dist < rayonPerimetre)
            {
                // trop proche
                etat = Etat.Fuite;
                running = Running.Course;
                destination = FindEscapePosition(Vu.position);
            }
            else if (SimpleMath.IsEncadré(dist, rayonPerimetre))
            {
                // pile à la bonne distance
                etat = Etat.Attend;
                running = Running.Arret;
                CalculeRotation(Vu.position);
                MoveAmount = Vector3.zero;
                AnimationStop();
            }
            else
            {
                // trop loin
                etat = Etat.Poursuite;
                running = Running.Marche;
                destination = Vu.position;
            }
        }

        private void Follow()
        {
            
        }
        
        private void Escape()
        {
            
        }

        // Prend la position la plus proche du bot parmi toutes
        // celles au périmètre (le cercle) du "Vu"
        private Vector3 FindEscapePosition(Vector3 centre)
        {
            timeLastFindDest = Time.time;

            (Vector3 bestDest, float minDist) res = (Vector3.zero, rayonPerimetre*2.1f);
            for (int degre = 0; degre < 360; degre += 10)
            {
                Vector3 pos = centre + rayonPerimetre * 1.1f * Calcul.Direction(degre);
                float dist = Calcul.Distance(Tr.position, pos);

                if (dist < res.minDist && capsule.CanIPass(Tr.position,
                    Calcul.Diff(pos, Tr.position), dist))
                {
                    // nouvelle meilleur destination
                    res = (pos, dist);
                }
            }

            return res.bestDest;
        }
        
        // bloqué
        protected override void WhenBlock()
        {
            //AmountRotation = 180;
        }
    }
}
