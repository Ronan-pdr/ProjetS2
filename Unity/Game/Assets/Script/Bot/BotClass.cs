using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Script;

public class BotClass : Humanoide
{

    protected void StartBot()
    {
        Tr.parent = BotManager.Instance.transform;
        
        maxHealth = 100;
    }

    protected void UpdateBot()
    {
        UpdateHumanoide();
    }

    protected override void Die()
    {
        enabled = false;
        BotManager.Instance.Die(gameObject);
    }
}
