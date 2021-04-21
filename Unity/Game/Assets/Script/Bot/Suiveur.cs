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
        
        // variable relative à la capsule
        private HumanCapsule capsule;
        
        // destination
        private Vector3 destination;


        // ------------ Constructeurs ------------
        private void Awake()
        {
            AwakeBot();
        }

        private void Start()
        {
            // tout le monde le fait pour qu'il soit parenter
            StartBot();
            
            master = MasterManager.Instance;
            etat = Etat.Attend;
            
            // récupérer les côtes des bots pour les ray
            capsule = MasterManager.Instance.GetHumanCapsule();
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
                
                // il recalcule sa rotation tous les 0.2f secondes
                if (Time.time - LastCalculRotation > 0.2f)
                {
                    CalculeRotation(destination);
                }
            }

            if (SimpleMath.Abs(AmountRotation) > 0)
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
                destination = FindEscapePosition(Vu.position);
                NearAndFar(Calcul.Distance(Tr.position, destination));
            }
            else if (SimpleMath.IsEncadré(dist, rayonPerimetre))
            {
                // pile à la bonne distance
                etat = Etat.Attend;
                CalculeRotation(Vu.position);
                MoveAmount = Vector3.zero;
                AnimationStop();
            }
            else
            {
                // trop loin
                etat = Etat.Poursuite;
                destination = Vu.position;
                SetMoveAmount(Vector3.forward, WalkSpeed);
                ActiverAnimation("Course");
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

            (Vector3 bestDest, float minDist) res = (Vector3.zero, rayonPerimetre*2.5f);
            for (int degre = 0; degre < 360; degre += 10)
            {
                Vector3 pos = centre + rayonPerimetre * Calcul.Direction(degre);
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
    }
}
