using System.Collections;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Manager;
using TMPro;
using UnityEngine;

public class PlayerInfoTab : MonoBehaviour
{
    // ------------ SerializedField ------------
    
    [SerializeField] private TMP_Text text;
    
    // ------------ Attributs ------------
    private PlayerClass player;
    private int limitation = 15;
    private string playerName;

    // ------------ Constructeurs ------------
    public void Set(PlayerClass value)
    {
        player = value;
        playerName = player.name;
    }
    
    // ------------ Méthodes ------------
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