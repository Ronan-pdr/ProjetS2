using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class PlayerClass : Humanoide, IDamagable
{
    //Avancer
    protected string touchAvancer = "z";
    protected string touchReculer = "s";
    protected string touchDroite = "d";
    protected string touchGauche = "q";
    
    //Sprint
    protected string touchSprint = "left shift";
    
    //Jump
    protected string touchJump = "space";
    
    //Look
    private float verticalLookRotation;
    private float mouseSensitivity = 3f;
    [SerializeField] protected Transform cameraHolder;
    
    //Photon
    protected PhotonView PV;

    //GamePlay
    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;
    

    protected void AwakePlayer()
    {
        Rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        Tr = GetComponent<Transform>();
             
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    protected void StartPlayer()
    {
        if (!PV.IsMine) // On veut détruire les caméras qui ne sont pas les tiennes
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
        
        FixedUpdateHumanoide();
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

        float speed = AnimationMove(xMov, zMov, walkSpeed);

        Vector3 moveDir = new Vector3(xMov, 0, zMov);

        MoveHumanoide(moveDir, speed);
    }

    private float AnimationMove(int xMov, int zMov, float speed)
    {
        if (xMov == 1) // On a décidé que l'aniation de la marche sur les côtés avaient la priorité
            SearchAnimation(touchDroite);
        else if (xMov == -1)
            SearchAnimation(touchGauche);
        else if (zMov == 1)
        {
            if (Input.GetKey(touchSprint)) // On ne peut seulement sprinter lorsque l'on avance 
            {
                speed = sprintSpeed;
                SearchAnimation(touchSprint);
            }
            else
                SearchAnimation(touchAvancer);
        }
        else if (zMov == -1)
            SearchAnimation(touchReculer);

        return speed;
    }

    private void Jump()
    {
        if (Input.GetKey(touchJump))
        {
            JumpHumanoide();
        }
    }

    void Look()
    {
        Tr.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

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
    void RPC_TakeDamage(float damage) // runs sur tous les ordis mais grâce à '!PV.IsMine', seul la victime subit les dommages
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
