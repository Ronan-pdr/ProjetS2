using System.Collections.Generic;
using Photon.Realtime;
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
            Statique,
            Follow,
            Escape,
            Searching,
            Looking
        }

        private Etat etat;
        
        // ------------ Attributs ------------
        
        private (Chasseur chasseur, Vector3 position) Vu;

        // temps
        private float timeManage;

        private static float rayonPerimetre = 25;
        
        // destination
        private Vector3 _whereToLookAt;
        
        // ------------ Setter ------------

        private void SetEscape()
        {
            running = Running.Course;
            _whereToLookAt = FindEscapePosition(Vu.position);
            etat = Etat.Escape;
        }

        private void SetStatique()
        {
            running = Running.Arret;
            _whereToLookAt = Vu.position;
            etat = Etat.Statique;
        }

        private void SetSearching()
        {
            running = Running.Marche;
            _whereToLookAt = Vu.position;
            etat = Etat.Searching;
        }

        private void SetFollow()
        {
            running = Running.Marche;
            _whereToLookAt = Vu.position;
            etat = Etat.Follow;
        }


        // ------------ Constructeurs ------------
        
        protected override void AwakeBot()
        {}

        protected override void StartBot()
        {
            running = Running.Arret;
            etat = Etat.Looking;
        }
    
        // ------------ Update ------------
        protected override void UpdateBot()
        {
            Tourner();
            GestionRotation(_whereToLookAt, 0.3f);
            
            if (Time.time - timeManage < 0.5f)
            {
                // il update son comportement sous tous les certains temps
                return;
            }

            timeManage = Time.time;

            if (Vu.chasseur is null)
            {
                // il n'a encore vu personne
                etat = Etat.Looking;
                SearchChasseurWithVision();
            }
            else if (IsInMyVision(Vu.chasseur))
            {
                // un chasseur est dan sma vision
                UpdateWhenChasseurInMyVision();
            }
            else
            {
                // a perdu de vue le chasseur
                if (etat == Etat.Escape)
                {
                    // s'il fuit c'est normal
                }
                else
                {
                    // il faut tenter de le retrouver
                    SetSearching();
                }
            }
            
            Debug.Log($"etat = {etat}");
        }

        private void UpdateWhenChasseurInMyVision()
        {
            // ...donc j'update sa position
            Vu.position = Vu.chasseur.transform.position;
            
            float dist = Calcul.Distance(Tr.position, Vu.position, Calcul.Coord.Y);
                    
            if (SimpleMath.IsEncadré(dist, rayonPerimetre, 1f))
            {
                // est pile à la bonne position
                SetStatique();
            }
            else if (dist < rayonPerimetre)
            {
                // trop proche
                SetEscape();
            }
            else
            {
                // est trop loin
                SetFollow();
            }
            
            CalculeRotation(_whereToLookAt);
        }

        // ------------ Méthodes ------------
        private void SearchChasseurWithVision()
        {
            List<PlayerClass> vus = GetPlayerInMyVision(TypePlayer.Chasseur);

            if (vus.Count > 0)
            {
                // Il a désormais un chasseur dans sa vision
                PlayerClass player = vus[0];
                
                Vu.chasseur = (Chasseur) player;
                Vu.position = player.transform.position;
                UpdateWhenChasseurInMyVision();
            }
        }

        // Prend la position la plus proche du bot parmi toutes
        // celles au périmètre (le cercle) du "Vu"
        private Vector3 FindEscapePosition(Vector3 centre)
        {
            (Vector3 bestDest, float minDist) res = (Vector3.zero, rayonPerimetre*2.1f);
            for (int degre = 0; degre < 360; degre += 10)
            {
                Vector3 pos = centre + rayonPerimetre * 1f * Calcul.Direction(degre);
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
        
        // ------------ Event ------------
        
        // bloqué
        protected override void WhenBlock()
        {
            AmountRotation = 180;
            running = Running.Arret;
        }
    }
}
