using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Arme
{
    protected void Shoot()
    {
        /*Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Le '?' est un peu comme un if local
            Debug.Log("I hit the " + hit.collider.name);
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(armeInfo.damage);
        }*/

        float alpha = cameraHoder.rotation.x;
        float beta = controller.transform.rotation.y;

        float dirX = SimpleMath.Sin(beta);
        float dirY = -SimpleMath.Sin(alpha);
        float dirZ = SimpleMath.Cos(alpha) * SimpleMath.Cos(beta);

        Vector3 direction = new Vector3(dirX, dirY, dirZ);
        //TeteChercheuse.VecteurCollision(controller, cam.gameObject.transform.position, direction, armeInfo.portéeAttaque);
    }
}
