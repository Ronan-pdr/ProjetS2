using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("Ground"))
        {
            Game.PlayerChassé.Ground = true;
            Debug.Log("I hit the ground !");
        }
    }
}
