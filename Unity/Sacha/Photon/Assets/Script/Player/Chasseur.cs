﻿using System;
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
    private void Awake()
    {
        AwakePlayer();
    }

    void Start()
    {
        StartPlayer();
    }
        
    void Update()
    {
        ManipulerArme();
            
        UpdateHumanoide();
        UpdatePlayer();
    }

    private void FixedUpdate()
    {
        FixedUpdatePlayer();
    }

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
        if (Input.GetMouseButtonDown(0))
        { 
            armes[armeIndex].Use();
        }
    }
        
    private void EquipItem(int index) // index supposé valide
    {
        // Le cas où on essaye de prendre l'arme qu'on a déjà
        if (index == previousArmeIndex)
            return;
            
        // C'est le cas où on avait pas encore d'arme précédente
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    //Animation
    protected override void SearchAnimation(string touche)
    {}
}