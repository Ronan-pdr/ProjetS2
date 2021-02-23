using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Arme
{
    protected void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Le '?' est un peu comme un if local
            Debug.Log("I hit the " + hit.collider.name);
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(armeInfo.damage);
        }
    }
}
