using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Arme
{
    public override void Use()
    {
        /*Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Le '?' est un peu comme un if local
            Debug.Log("I hit the " + hit.collider.name);
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(armeInfo.damage);
        }*/

        float rotCam = cameraHoder.eulerAngles.x;
        float rotChasseur = controller.transform.eulerAngles.y;

        Vector3 rotation = new Vector3(rotCam, rotChasseur, 0);
        
        TeteChercheuse teteChercheuse = TeteChercheuse.Initialisation(cameraHoder.gameObject.transform.position);
        teteChercheuse.VecteurCollision(controller, rotation, armeInfo.GetPortéeAttaque());
    }
}
