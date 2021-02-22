using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Script
{
    public class Chassé : Player
    {
        private static Vector3 distanceCamera = new Vector3(0, 0.5f, -2);

        void Start()
        {
            
        }
        
        /*public Chassé(Transform tr, Rigidbody rb, Transform cam)
        {
            Tr = tr;
            Rb = rb;
            Cam = cam;
            Cam.position = tr.position + distanceCamera;
        }*/
        
        void Update()
        {
            Upd();
            FixedUpd();
        }
    }
}