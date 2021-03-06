using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Arme
{
    private float speedBalls = 20f;

    public override void UtiliserArme()
    {
        float rotCam = cameraHolder.eulerAngles.x;
        float rotChasseur = controller.transform.eulerAngles.y;

        Vector3 rotation = new Vector3(rotCam, rotChasseur, 0);
        
        BalleFusil.Tirer(cameraHolder.gameObject.transform.position,
            controller, rotation, armeInfo, speedBalls);
    }
}
