using UnityEngine;
using Script.TeteChercheuse;

namespace Script.DossierArme
{
    public class Gun : Arme
    { 
        public override void UtiliserArme()
        {
            float rotCam = cameraHolder.eulerAngles.x;
            float rotChasseur = controller.transform.eulerAngles.y;

            Vector3 rotation = new Vector3(rotCam, rotChasseur, 0);
        
            BalleFusil.Tirer(cameraHolder.gameObject.transform.position,
                controller, rotation, armeInfo);
        }
    }
}

