using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Script;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BotClass : Humanoide
{
    private BotManager botManager; // instancié lorsque le bot est créé dans son BotManager
    
    // Getter
    public string GetName() => name;

    // Setter
    public void SetOwnBotManager(BotManager _botManager)
    {
        botManager = _botManager;
    }
    
    // cette fonction indique si un bot est ton bot
    public bool IsMyBot()
    {
        return botManager != null;
    }

    protected void StartBot()
    {
        SetRbTr();

        PV = GetComponent<PhotonView>();

        name = MasterManager.Instance.GetNameBot(PV.Owner);
        Debug.Log(name);

        if (botManager == null) // cela veut dire que c'est pas cet ordinateur qui a créé ces bots ni qui les contrôle
            Tr.parent = MasterManager.Instance.GetDossierOtherBot(); // le parenter dans le dossier qui contient tous les bots controlés par les autres joueurs
        else
            Tr.parent = botManager.transform; // le parenter dans ton dossier de botManager
        
        
        maxHealth = 100;
        StartHuman();
    }

    protected void UpdateBot()
    {
        UpdateHumanoide();
    }

    protected override void Die()
    {
        enabled = false;
        botManager.Die(gameObject);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        
    }
}
