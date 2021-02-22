using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Player : Humanoide
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
    private float jumpForce = 3000f;
    
    //Look
    private float verticalLookRotation;
    private float cameraHolder;
    private float mouseSensitivity = 3f;
    
    //Photon
    private PhotonView PV;

    //GamePlay
    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        // On veut détruire les caméras qui ne sont pas les tiennes
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(Rb);
        }
    }

    protected override void Upd()
    {
        if (!PV.IsMine)
            return;
        
        Look();
        Move();
        Jump();
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
            Debug.Log("I jump !");
            Rb.AddForce(transform.up * jumpForce); // transform.up = new Vector3(0, 0, 0)
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    protected override void FixedUpd()
    {
        if (!PV.IsMine)
            return;
        
        Rb.MovePosition(Rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
