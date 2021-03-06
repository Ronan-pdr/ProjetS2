using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Chasseur : PlayerClass
{
    // Relatif aux armes
    [SerializeField] private Arme[] armes;
    private int armeIndex;
    private int previousArmeIndex = -1;
    
    //Getter
    private void Awake()
    {
        AwakePlayer();
    }

    void Start()
    {
        maxHealth = 100;
        StartPlayer();
        
        EquipItem(0);
    }
        
    void Update()
    {
        if (!PV.IsMine)
            return;
        
        if (PauseMenu.PauseMenu.isPaused)
        {
            moveAmount = Vector3.zero;
            return;
        }

        ManipulerArme();
            
        UpdatePlayer();
    }

    private void FixedUpdate()
    {
        FixedUpdatePlayer();
    }
    

    //GamePlay
    
    private void ManipulerArme()
    {
        //changer d'arme avec les numéros
        for (int i = 0; i < armes.Length; i++)
        {
            if (Input.GetKey((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        //changer d'arme avec la molette
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            EquipItem(SimpleMath.Mod(previousArmeIndex + 1, armes.Length));
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            EquipItem(SimpleMath.Mod(previousArmeIndex - 1, armes.Length));
        }

        //tirer
        if (Input.GetMouseButton(0))
        { 
            armes[armeIndex].Use();
        }
    }
        
    public void EquipItem(int index) // index supposé valide
    {
        // Le cas où on essaye de prendre l'arme qu'on a déjà
        if (index == previousArmeIndex)
            return;
            
        // C'est le cas où on avait déjà une arme, il faut la désactiver
        if (previousArmeIndex != -1)
        {
            armes[previousArmeIndex].armeObject.SetActive(false);
        }

        previousArmeIndex = armeIndex;
            
        armeIndex = index;
        armes[armeIndex].armeObject.SetActive(true);

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", armeIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    /*public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        
    }*/

    protected override void Die()
    {
        
    }

    //Animation
    protected override void SearchAnimation(string touche)
    {}
}