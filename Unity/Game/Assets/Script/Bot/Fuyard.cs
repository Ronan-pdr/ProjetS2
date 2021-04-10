using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Test;
using Script.TeteChercheuse;
using Script.Tools;
using UnityEngine;

namespace Script.Bot
{
    public class Fuyard : BotClass, ISReceveurRayGaz
    {
        // Etat
        protected enum Etat
        {
            Attend,
            FuiteSansPlan,
            Fuite
        }

        private Etat etat = Etat.Attend;
        
        // variables relatives à la caméra des chassés
        private static float AngleCamera; // correspond au "Field Of View"
        private static Vector3 PositionCamera; // correspond à la distance séparant le "cameraHolder" de la "camera" de type "Camera"
        
        // cette liste va contenir la position des chasseurs lorsque le bot les a "vu"
        // si le bot n'en a pas vu, la liste est vide
        private List<(Chasseur chasseur, Vector3 position)> Vus = new List<(Chasseur chasseur, Vector3 position)>();
        
        // masterManager
        private MasterManager master;
        
        // fuite
        private List<Vector3> planFuite;
        
        /*private float tempsMaxFuite = 3f;
        private float tempsRestantFuite;
        private float distanceFuite;*/
        
        // variable relative à sa capsule
        private (float hauteur, float rayon) capsule;

        // Setter
        public override void SetBot(CrossPoint crossPoint)
        {}
        
        private void Awake()
        {
            AwakeBot();
        }

        private void Start()
        {
            CapsuleCollider cap = GetComponent<CapsuleCollider>();
            float scale = cap.transform.localScale.y;
            capsule.hauteur = cap.height * scale;
            capsule.rayon = cap.radius * scale;
            
            rotationSpeed = 800;
            
            StartBot(); // tout le monde le fait pour qu'il soit parenter
            
            master = MasterManager.Instance;

            //distanceFuite = SprintSpeed * tempsMaxFuite;
        }

        public static void SetInfoCamera(PlayerClass player)
        {
            Camera camera = player.GetComponentInChildren<Camera>();
            
            //AngleCamera = camera.fieldOfView;
            AngleCamera = 80;

            PositionCamera = new Vector3(0, 1.4f, 0.3f);
        }

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
                IsChasseurInMyVision();
            }
        }

        private void FixedUpdate()
        {
            if (IsMyBot())
            {
                MoveEntity();
            }
        }

        private void IsChasseurInMyVision()
        {
            int nChasseur = master.GetNbChasseur();
            Vector3 positionCamera = Tr.position + Tr.TransformDirection(PositionCamera);
            
            for (int i = 0; i < nChasseur; i++)
            {
                Vector3 positionChasseur = master.GetChasseur(i).transform.position;

                float angleY = Calcul.Angle(Tr.eulerAngles.y, positionCamera,
                    positionChasseur, Calcul.Coord.Y);

                if (SimpleMath.Abs(angleY) < AngleCamera) // le chasseur est dans le champs de vision du bot ?
                {
                    Ray ray = new Ray(positionCamera, Calcul.Diff(positionChasseur, positionCamera));

                    if (Physics.Raycast(ray, out RaycastHit hit)) // y'a t'il aucun obstacle entre le chasseur et le bot ?
                    {
                        if (hit.collider.GetComponent<Chasseur>()) // si l'obstacle est le chasseur alors le bot "VOIT" le chasseur
                        {
                            NewVu(hit.collider.GetComponent<Chasseur>());
                        }
                    }
                }
            }
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
            
            Debug.Log($"J'ai vu {Vus.Count} chasseurs différents");
            
            // cherche un plan bien rodé vers une destination stratégique
            Vector3 dest = BotManager.Instance.GetGoodSpot(this, Vus[0].position);
            RayGaz.GetPath(Tr.position, dest, this);

            etat = Etat.FuiteSansPlan;
        }

        public void RecepRayGaz(List<Vector3> path)
        {
            if (path.Count == 0)
            {
                // n'a pas de destination, n'a vraiment pas de plan...
                //etat = Etat.Attend;
            } 
            else
            {
                // part en cavale
                planFuite = path;
                etat = Etat.Fuite;
            
                foreach (Vector3 p in planFuite)
                {
                    TestRayGaz.CreatePointPath(p);
                }
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
            Vector3 dest = planFuite[len - 1];
            
            // s'il a finit une étape de son plan
            if (Calcul.Distance(Tr.position, dest, Calcul.Coord.Y) < 1f)
            {
                planFuite.RemoveAt(len - 1);
                
                if (planFuite.Count == 0) // il finit sa cavale,...
                {
                    MoveAmount = Vector3.zero; // ...il s'arrête...
                    etat = Etat.Attend;
                    Vus.Clear();
                    AnimationStop();
                    return; // ...et ne fait rien d'autre
                }
                
                AmountRotation = Calcul.Angle(Tr.eulerAngles.y, Tr.position, planFuite[len-2], Calcul.Coord.Y);
            }
            
            if (SimpleMath.Abs(AmountRotation) > 0)
                Tourner();
            
            // set sa vitesse actuel (se déplace toujours droit devant lui)
            SetMoveAmount(Vector3.forward, SprintSpeed);
            
            // animation
            anim.enabled = true;
            anim.Play("Course");
        }
    }
}