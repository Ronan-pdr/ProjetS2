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
    
    //Avancer
    protected float walkSpeed = 3f;
    protected float sprintSpeed = 5f;
    protected Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private float smouthTime = 0.15f;
    
    //Jump
    protected float jumpForce = 200f;
    
    
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

        if (!Input.anyKey)
            AnimationStop();
    }

    protected void FixedUpdateHumanoide()
    {
        //Déplace le corps du human grâce à moveAmount précédemment calculé
        Rb.MovePosition(Rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    protected void MoveHumanoide(Vector3 moveDir, float speed) // moveDir doit être de la forme (1, 0, 0), (0, 0, -1), (1, 0, 1)... mais pas de 1 sur y
    {
        moveAmount = Vector3.SmoothDamp(moveAmount,
            moveDir * speed,
            ref smoothMoveVelocity, smouthTime);
    }

    protected void JumpHumanoide()
    {
        if (Grounded)
        {
            Rb.AddForce(transform.up * jumpForce); // transform.up = new Vector3(0, 1, 0)
            Grounded = false;
        }
    }

    protected void Die()
    {
        playerManager.Die();
    }

    //Animation
    [SerializeField] protected Animator anim;
    protected abstract void SearchAnimation(string touche);

    private void AnimationStop()
    {
        anim.enabled = false;
    }
}
