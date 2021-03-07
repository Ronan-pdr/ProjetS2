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

    protected void AwakeBot()
    {
        AwakeHuman();
    }

    protected void StartBot()
    {
        SetRbTr();
        
        maxHealth = 100;
        StartHuman(); // vie

        name = MasterManager.Instance.GetNameBot(PV.Owner);
        Debug.Log(name);

        if (botManager == null) // cela veut dire que c'est pas cet ordinateur qui a créé ces bots ni qui les contrôle
            Tr.parent = MasterManager.Instance.GetDossierOtherBot(); // le parenter dans le dossier qui contient tous les bots controlés par les autres joueurs
        else
            Tr.parent = botManager.transform; // le parenter dans ton dossier de botManager
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
    
    // réception des hash
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.Owner.Equals(targetPlayer)) // si c'est pas toi la target, tu ne changes rien
            return;
        
        // point de vie -> TakeDamage (Humanoide)
        // Tout le monde doit faire ce changement (trop compliqué de retrouvé celui qui l'a déjà fait)
        changedProps.TryGetValue("PointDeVieBot", out object life);

        if (life != null)
        {
            string deuxInfos = (string) life;
            int len = deuxInfos.Length;
            string nameTarget = deuxInfos.Substring(0, len - 3);

            if (name == nameTarget) // parce que chaque joueur contrôle plusieurs bot, il faut donc faire une deuxième vérification
            {
                int vie = int.Parse(deuxInfos.Substring(len - 3, 3));
                            
                Debug.Log($"Update de la vie de {name}");
                currentHealth = (int)vie;
            }
            
        }
    }
}
