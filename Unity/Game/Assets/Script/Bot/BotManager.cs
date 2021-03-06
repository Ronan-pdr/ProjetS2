using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;


public class BotManager : MonoBehaviour
{
    public static BotManager Instance;
    private List<BotClass> Bots;
    
    // Est ce que la liste a déjà été instancié
    private bool DejaFait;
    private float debutJeu; // le moment exacte ou le jeu commence
    private float ecart = 0.5f; // Au bout de combien de temps il instancie les listes

    void Start()
    {
        Instance = this;
        Bots = new List<BotClass>();
        
        if (!PhotonNetwork.IsMasterClient) // Seule le masterClient créé les bots
            return;

        Player[] players = PhotonNetwork.PlayerList;
        int nBot = 12;

        int nBotMax = CrossManager.Instance.GetNumberPoint();
        if (nBot > nBotMax) //Dépassement du nombre de point
        {
            nBot = nBotMax;
        }

        for (int i = 0; i < nBot; i++) // Instancier et ranger tous les bots
        {
            BotRectiligne bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "BotRectiligne"),
                Vector3.zero, Quaternion.identity).GetComponent<BotRectiligne>();
            Bots.Add(bot);

            bot.transform.position += CrossManager.Instance.GetPosition(i);
        }

        DejaFait = true;
    }

    private void Update()
    {
        if (DejaFait || Time.time - debutJeu < ecart) // attend que le masterClient ait créé et classé les bots
            return;

        foreach (BotClass bot in GetComponentsInChildren<BotClass>())
        {
            Bots.Add(bot);
        }

        DejaFait = true;
    }

    public void Die(GameObject bot)
    {
        Bots.Remove(bot.GetComponent<BotClass>());
        
        PhotonNetwork.Destroy(bot.gameObject);
        enabled = false;
    }
}