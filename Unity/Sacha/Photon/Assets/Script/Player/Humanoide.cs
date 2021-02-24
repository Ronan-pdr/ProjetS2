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
    
    //Mouvement
    protected float walkSpeed = 3f;
    protected float sprintSpeed = 5f;
    protected float jumpForce = 200f;
    protected Vector3 moveAmount;
    
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
