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
    protected float lastChangementEtat; // La dernière qu'on a changé entre assis et lever
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
    
    //Look
    private float verticalLookRotation; 
    private float mouseSensitivity = 3f;
    [SerializeField] protected Transform cameraHolder;
    
    //Photon
    protected PhotonView PV;
    private Player _player; // Sa valeur n'est instancié que 

    public Player GetPlayer() => _player;

    //Rassembler les infos
    protected Transform masterManager;
    
    
    protected void AwakePlayer()
    {
        masterManager = MasterManager.Instance.transform;
        transform.parent = masterManager;
        
        SetRbTr();
        
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

        if (PV.IsMine)
        {
            _player = PhotonNetwork.LocalPlayer;
            
            Hashtable hash = new Hashtable();
            hash.Add("player", _player);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
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
            etatDebAssAcc = 0; // il ne reste pas accroupi lorqu'il se déplace mais pas tout droit
        }

        Vector3 moveDir = new Vector3(xMov, 0, zMov);

        SetMoveAmount(moveDir, speed);
    }
    
    private void Jump()
    {
        if (Input.GetKey(touchJump) && Grounded && etatDebAssAcc == 0) //il faut qu'il soit debout
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
    
    //GamePlay
    
    // Regarde dans gamePlay Humanoide si jamais tu comprends pas
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.Owner.Equals(targetPlayer))
            return;
        
        // Set '_player' de type Player (Photon)
        if (!PV.IsMine) // tu l'as déjà fait avec ton point de vue
        {
            changedProps.TryGetValue("player", out object p);

            if (p != null)
            {
                _player = (Player)p;
            }
        }
        
        // arme du chasseur
        if (this is Chasseur && !PV.IsMine) // ça ne doit pas être ton point de vie puisque tu l'as déjà fait
        {
            changedProps.TryGetValue("itemIndex", out object indexArme);

            if (indexArme != null)
            {
                Chasseur chasseur = (Chasseur) this;
                chasseur.EquipItem((int)indexArme);
            }
        }

        // point de vie
        if (!PhotonNetwork.IsMasterClient)
        {
            changedProps.TryGetValue("PointDeVie", out object life);

            if (life != null)
            {
                currentHealth = (int)life;
            }
        }
    }
}
