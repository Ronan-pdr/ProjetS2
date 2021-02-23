using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerClass : Humanoide, IDamagable
{
    //Avancer
    private float walkSpeed = 3f;
    private string touchAvancer = "z";
    private string touchReculer = "s";
    private string touchDroite = "d";
    private string touchGauche = "q";
    
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private float smouthTime = 0.15f;
    
    //Sprint
    private float sprintSpeed = 5f;
    private KeyCode touchSprint = KeyCode.LeftShift;
    
    //Jump
    private string touchJump = "space";
    private float jumpForce = 200f;
    
    //Look
    private float verticalLookRotation;
    private float mouseSensitivity = 3f;
    [SerializeField] protected Transform cameraHolder;
    [SerializeField] protected Vector3 distanceCamera;
    
    //Photon
    protected PhotonView PV;

    //GamePlay
    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;
    

    protected void AwakePlayer()
    {
        Rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    protected void StartPlayer()
    {
        if (PV.IsMine) // Placer la caméra en fonction de ta classe (chasseur/chassé) 
        {
            cameraHolder.position += distanceCamera;
        }
        else // On veut détruire les caméras qui ne sont pas les tiennes
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(Rb);
        }
    }

    protected void UpdatePlayer()
    {
        if (!PV.IsMine)
            return;
        
        Look();
        Move();
        Jump();
    }
    
    protected void FixedUpdatePlayer()
    {
        if (!PV.IsMine)
            return;
        
        //Déplce le corps du joueur grâce à moveAmount précédemment calculé
        Rb.MovePosition(Rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
    
    private void Move()
    {
        int zMov = 0;
        if (Input.GetKey(touchAvancer))
            zMov++;
        if (Input.GetKey(touchReculer))
            zMov--;
        int xMov = 0;
        if (Input.GetKey(touchDroite))
            xMov++;
        if (Input.GetKey(touchGauche))
            xMov--;

        Vector3 moveDir = new Vector3(xMov, 0, zMov);

        moveAmount = Vector3.SmoothDamp(moveAmount,
            moveDir * (Input.GetKey(touchSprint) ? sprintSpeed : walkSpeed),
            ref smoothMoveVelocity, smouthTime);
    }

    private void Jump()
    {
        if (Input.GetKey(touchJump) && Grounded)
        {
            Rb.AddForce(transform.up * jumpForce); // transform.up = new Vector3(0, 0, 0)
            Grounded = false;
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -70f, 70f);

        cameraHolder.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    
    public void TakeDamage(float damage) // Runs sur l'ordinateur du shooter
    {
        Debug.Log("took damage : " + damage);
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    
    [PunRPC]
    void RPC_TakeDamage(float damage) // runs sur tous les ordis mais grâce à '!PV.IsMine', seuk la victime subit les dommages
    {
        if (!PV.IsMine)
            return;
        
        Debug.Log("took damage " + damage);
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        throw new NotImplementedException();
    }
}
