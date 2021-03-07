using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class Humanoide : Entity
{
    //Etat
    protected bool Grounded;

    //Avancer
    protected const float walkSpeed = 3f;
    protected const float sprintSpeed = 5f;

    //Jump
    private const float jumpForce = 200f;
    
    //GamePlay
    protected int maxHealth;
    protected int currentHealth;
    
    // photon
    protected PhotonView PV;
    
    //Getter
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public PhotonView GetPV() => PV;
    public Player GetPlayer() => PV.Owner;
    
    
    public void SetGroundedState(bool grounded)
    {
        Grounded = grounded;
    }

    protected void StartHuman()
    {
        currentHealth = maxHealth;
    }

    private void PotentielleMort()
    {
        // Mourir de chute
        if (transform.position.y < -10f)
        {
            Die();
        }
        
        // Mourir point de vie
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected void UpdateHumanoide()
    {
        PotentielleMort();
    }
    
    //Déplacment

    protected void JumpHumanoide()
    {
        Rb.AddForce(transform.up * jumpForce); // transform.up = new Vector3(0, 1, 0)
        Grounded = false;
    }

    //Animation
    [SerializeField] protected Animator anim;

    protected void AnimationStop()
    {
        anim.enabled = false;
    }
    
    //GamePlay
    public void TakeDamage(int damage) // Seul le masterClient active cette fonction
    {
        currentHealth -= damage;
        
        Hashtable hash = new Hashtable();

        if (this is PlayerClass)
        {
            hash.Add("PointDeViePlayer", currentHealth);
        }
        else
        {
            BotClass bot = (BotClass) this;
        }

        GetPlayer().SetCustomProperties(hash);
    }

    protected abstract void Die();
}
