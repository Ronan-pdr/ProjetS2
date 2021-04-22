using System;
using System.Collections;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Manager;
using TMPro;
using UnityEngine;

public class PlayerInfoTab : MonoBehaviour
{
    // ------------ Attributs ------------
    
    private PlayerClass player;
    private int limitation = 15;
    private string playerName;
    private TMP_Text text;

    // ------------ Constructeurs ------------
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Set(PlayerClass value)
    {
        player = value;
        playerName = player.name;
    }
    
    // ------------ Upadte ------------
    void Update()
    {
        if (player is null)
            return;
        
        int vie = player.GetCurrentHealth();
        if (playerName.Length < limitation)
        {
            for (int i = playerName.Length - 1; i < limitation; i++)
            {
                playerName += " ";
            }
        }
        text.text = playerName;
        text.text += vie <= 0 ? "Dead" : vie + " / " + player.GetMaxHealth();
    }
}