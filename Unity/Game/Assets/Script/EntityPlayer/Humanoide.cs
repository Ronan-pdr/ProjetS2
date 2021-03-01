using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class Humanoide : Entity
{
    protected bool Grounded;
    protected PlayerManager playerManager;
    
    //Avancer
    protected float walkSpeed = 3f;
    protected float sprintSpeed = 5f;

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

    protected void JumpHumanoide()
    {
        Rb.AddForce(transform.up * jumpForce); // transform.up = new Vector3(0, 1, 0)
        Grounded = false;
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
    
    //Fonctions communes aux chassées ainsi qu'aux bots
    
    
}
