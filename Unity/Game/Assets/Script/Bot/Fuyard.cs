using System;

using Script.DossierPoint;
using Script.EntityPlayer;
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
        
        // variables relatives à la caméra des chassés
        private static float AngleCamera; // correspond au "Field Of View"
        private static Vector3 PositionCamera; // correspond à la distance séparant le "cameraHolder" de la "camera" de type "Camera"

        private static bool FeuVert = false;
        
        // masterManager
        private MasterManager master;
        
        // Setter
        public override void SetBot(CrossPoint crossPoint)
        {}
        
        private void Awake()
        {
            AwakeBot();
        }

        private void Start()
        {
            StartBot(); // tout le monde le fait pour qu'il soit parenter
            
            master = MasterManager.Instance;
        }

        public static void SetInfoCamera(PlayerClass player)
        {
            Camera camera = player.GetComponentInChildren<Camera>();
            
            AngleCamera = camera.fieldOfView;

            PositionCamera = new Vector3(0, 1.4f, -1.6f);

            FeuVert = true;
        }

        private void Update()
        {
            if (!FeuVert)
                return;
            
            if (IsChasseurInMyVision())
                Fuir();
            else
                AnimationStop();
        }

        private bool IsChasseurInMyVision()
        {
            int nChasseur = master.GetNbChasseur();
            
            for (int i = 0; i < nChasseur; i++)
            {
                Vector3 positionChasseur = master.GetChasseur(i).transform.position;
                Vector3 positionCamera = Tr.position + PositionCamera;
                
                float angleY = Calcul.Angle(Tr.eulerAngles.y, positionCamera,
                    positionChasseur, Calcul.Coord.Y);

                if (SimpleMath.Abs(angleY) < AngleCamera) // le chasseur est dans le champs de vision du bot ?
                {
                    Ray ray = new Ray(positionCamera, Calcul.Diff(positionChasseur, positionCamera));
                    
                    if (Physics.Raycast(ray, out RaycastHit hit)) // y'a t'il aucun obstacle entre le chasseur et le bot ?
                    {
                        //Debug.Log($"I hit the {hit.collider.name}, angleY = {angleY}");
                    
                        if (hit.collider.GetComponent<Chasseur>())
                            return true;
                    }
                }
            }
            
            return false;
        }

        private void Fuir()
        {
            anim.enabled = true;
            anim.Play("Course");
        }
    }
}