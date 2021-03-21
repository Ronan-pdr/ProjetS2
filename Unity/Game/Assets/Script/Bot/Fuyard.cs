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
            int i;
            float angleY = 0;
            int nChasseur = master.GetNbChasseur();
            
            for (i = 0; i < nChasseur; i++)
            {
                angleY = Calcul.Angle(Tr.eulerAngles.y, Tr.position,
                    master.GetChasseur(i).transform.position, Calcul.Coord.Y);
                
                if (SimpleMath.Abs(angleY) < AngleCamera)
                    break;
            }

            if (nChasseur == 0 || i == nChasseur)
                return false;

            Vector3 position = Tr.position;
            
            float angleX = Calcul.Angle(Tr.eulerAngles.x, position,
                master.GetChasseur(i).transform.position, Calcul.Coord.X);

            Ray ray = new Ray(new Vector3(SimpleMath.DegreToRadian(angleX), SimpleMath.DegreToRadian(angleY), 0), PositionCamera + position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("I hit the " + hit.collider.name);

                return hit.collider.GetComponent<Chasseur>();
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