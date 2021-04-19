using System.Collections;
using System.Collections.Generic;
using Script.EntityPlayer;
using TMPro;
using UnityEngine;

public class PlayerInfoTab : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private MasterManager Instance = MasterManager.Instance;
    private PlayerClass player;
    private int limitation = 15;
    private string playerName;

    public void Set(PlayerClass value)
    {
        player = value;
        playerName = player.name;
    }
    void Update()
    {
        if (player == null)
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
