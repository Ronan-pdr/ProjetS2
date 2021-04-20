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

        private static float rayonPerimetre = 10;
        
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
            rotationSpeed = 600;
            
            // tout le monde le fait pour qu'il soit parenter
            StartBot();
            
            master = MasterManager.Instance;
            etat = Etat.Attend;
            
            // récupérer les côtes des bots pour les ray
            capsule = MasterManager.Instance.GetHumanCapsule();
        }
    
        // ------------ Méthodes ------------
        private void Update()
        {
            if (!IsMyBot())
                return;

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
                
                // il recalcule sa rotation tous les 0.5f
                if (Time.time - LastCalculRotation > 0.1f)
                {
                    CalculeRotation(destination);
                }
            }

            if (SimpleMath.Abs(AmountRotation) > 0)
                Tourner();
        }
        
        private void FixedUpdate()
        {
            if (MoveAmount == Vector3.zero)
            {
                AnimationStop();
            }
            else if (etat == Etat.Fuite)
            {
                ActiverAnimation("Course");
            }
            else
            {
                ActiverAnimation("Avant");
            }
            
            FixedUpdateBot();
        }

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
                etat = Etat.Fuite;
                destination = FindEscapePosition(Vu.position);
                SetMoveAmount(Vector3.forward, SprintSpeed);
            }
            else if (SimpleMath.IsEncadré(dist, rayonPerimetre))
            {
                etat = Etat.Attend;
                CalculeRotation(Vu.position);
                MoveAmount = Vector3.zero;
            }
            else
            {
                etat = Etat.Poursuite;
                destination = Vu.position;
                SetMoveAmount(Vector3.forward, WalkSpeed);
            }
        }

        private void Follow()
        {
            
        }
        
        private void Escape()
        {
            
        }

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

        protected override void FiniDeTourner()
        {}
    }
}
