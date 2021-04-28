using Script.Animation;
using Script.EntityPlayer;
using UnityEngine;
using Script.TeteChercheuse;

namespace Script.DossierArme
{
    public class Gun : Arme
    {
        // ------------ Serialize Field ------------
        
        [Header("Porteur")]
        [SerializeField] protected Chasseur porteur;
        
        // ------------ Méthode ------------
        public override void UtiliserArme()
        {
            anim.Set(HumanAnim.Type.Shoot);

            float rotCam = cameraHolder.eulerAngles.x;
            float rotChasseur = porteur.transform.eulerAngles.y;

            Vector3 rotation = new Vector3(rotCam, rotChasseur, 0);
        
            BalleFusil.Tirer(cameraHolder.gameObject.transform.position,
                porteur, rotation, armeInfo);
        }
    }
}

