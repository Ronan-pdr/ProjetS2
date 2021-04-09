using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.TeteChercheuse;
using Script.Tools;
using UnityEngine;

namespace Script.Bot
{
    public class Fuyard : BotClass
    {
        // Etat
        protected enum Etat
        {
            Attend,
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
        private float tempsMaxFuite = 3f;
        private float tempsRestantFuite;
        private float distanceFuite;
        
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
            
            rotationSpeed = 600;
            
            StartBot(); // tout le monde le fait pour qu'il soit parenter
            
            master = MasterManager.Instance;

            distanceFuite = SprintSpeed * tempsMaxFuite;
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
            IsChasseurInMyVision();

            if (etat == Etat.Fuite)
                Fuir();
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
        }

        protected override void FiniDeTourner()
        {} // lorsqu'il a fini de tourner, il ne fait rien de plus

        private void Fuir()
        {
            if (tempsRestantFuite < 0) // s'il a couru assez longtemps,...
            {
                MoveAmount = Vector3.zero; // ...il s'arrête
                etat = Etat.Attend;
                Vus.Clear();
                AnimationStop();
                return;
            }

            tempsRestantFuite -= Time.deltaTime; // décrémenter son temps de fuite
            
            if (SimpleMath.Abs(AmountRotation) > 0)
                Tourner();
            
            // set sa vitesse actuel (se déplace toujours droit devant lui)
            SetMoveAmount(new Vector3(0, 0, 1), SprintSpeed);
            
            // animation
            anim.enabled = true;
            anim.Play("Course");
        }
    }
}