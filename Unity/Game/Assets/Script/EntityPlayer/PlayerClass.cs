using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Script;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class PlayerClass : Humanoide
{
    // Etat
    protected string touchLeverAssoir = "c";
    protected float lastChangementEtat; // La dernière fois qu'on a changé de position entre assis et lever
    protected string touchAccroupi = "x";
    protected int etatDebAssAcc = 0; // debout -> 0 ; Assis -> 1 ; Acc -> 2
    
    //Avancer
    protected string touchAvancer = "z";
    protected string touchReculer = "s";
    protected string touchDroite = "d";
    protected string touchGauche = "q";
    
    //Sprint
    protected string touchSprint = "left shift";
    
    //Jump
    protected string touchJump = "space";
    private float lastJump; // le temps la dernière fois que le joueur a sauté
    private float periodeJump = 0.2f; // tous les combien de temps il peut sauter
    
    //Look
    private float verticalLookRotation; 
    private float mouseSensitivity = 3f;
    [SerializeField] protected Transform cameraHolder;

    //Rassembler les infos
    protected Transform masterManager;


    protected void AwakePlayer()
    {
        SetRbTr();
        
        // parenter
        masterManager = MasterManager.Instance.transform;
        Tr.parent = masterManager;
        
        // Le ranger dans la liste du MasterManager
        MasterManager.Instance.AjoutPlayer(this);
        
        //photon
        PV = GetComponent<PhotonView>();
    }

    protected void StartPlayer()
    {
        StartHuman();
        
        if (!PV.IsMine) 
        {
            Destroy(GetComponentInChildren<Camera>().gameObject); // On veut détruire les caméras qui ne sont pas les tiennes
            Destroy(Rb);
            
            
        }
    }

    protected void UpdatePlayer()
    {
        Look();
        Move();
        Jump();
        
        UpdateHumanoide();
    }
    
    protected void FixedUpdatePlayer()
    {
        if (!PV.IsMine)
            return;
        
        moveEntity();
    }
    
    private void Move()
    {
        if (etatDebAssAcc == 1) // assis
        {
            return;
        }
        
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

        float speed = walkSpeed;
        if (zMov == 1 && xMov == 0) // il faut qu'il avance tout droit pour sprinter
            speed = sprintSpeed;
        else if (xMov != 0 || zMov != 0) // en gros s'il se déplace
        {
            etatDebAssAcc = 0; // il ne reste pas accroupi lorqu'il se déplace pas tout droit
        }

        Vector3 moveDir = new Vector3(xMov, 0, zMov);

        SetMoveAmount(moveDir, speed);
    }
    
    private void Jump()
    {
        if (Input.GetKey(touchJump) && Time.time - lastJump > periodeJump && Grounded && etatDebAssAcc == 0) //il faut qu'il soit debout
        {
            JumpHumanoide();

            lastJump = Time.time;
        }
    }

    void Look()
    {
        Tr.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -50f, 30f);

        cameraHolder.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    
    // Communication par hash
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.Owner.Equals(targetPlayer)) // si c'est pas toi la target, tu ne changes rien
            return;
        
        // arme du chasseur -> EquipItem (Chasseur)
        if (this is Chasseur && !PV.IsMine) // ça ne doit pas être ton point de vie puisque tu l'as déjà fait
        {
            changedProps.TryGetValue("itemIndex", out object indexArme);

            if (indexArme != null)
            {
                Chasseur chasseur = (Chasseur) this;
                chasseur.EquipItem((int)indexArme);
            }
        }

        // point de vie -> TakeDamage (Humanoide)
        if (!PhotonNetwork.IsMasterClient) // c'est le masterClient qui contrôle les balles donc qui enlève les point de vies
        {
            changedProps.TryGetValue("PointDeViePlayer", out object life);

            if (life != null)
            {
                currentHealth = (int)life;
            }
        }
        
        // les morts
        if (!PV.IsMine) // c'est le mourant qui envoie le hash
        {
            changedProps.TryGetValue("MortPlayer", out object _player);

            if (_player != null)
            {
                Player player = (Player) _player;
                MasterManager.Instance.Die(player);
            }
        }
    }
}
