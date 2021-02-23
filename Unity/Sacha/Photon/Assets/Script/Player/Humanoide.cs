using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Humanoide : MonoBehaviour
{
    protected Transform Tr;
    protected Rigidbody Rb;
    protected bool Grounded;
    
    protected PlayerManager playerManager;
    
    public void SetGroundedState(bool grounded)
    {
        Grounded = grounded;
    }

    protected void UpdateHumanoide()
    {
        // Mourir de chute
        if (transform.position.y < -10f)
        {
            Die();
        }
    }
    
    protected void Die()
    {
        playerManager.Die();
    }
}
