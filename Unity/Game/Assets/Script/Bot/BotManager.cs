using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;


public class BotManager : MonoBehaviour
{
    private List<BotRectiligne> Bots;
    private Player[] players;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Bots = new List<BotRectiligne>();
        }
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            players = PhotonNetwork.PlayerList;
            int nBot = 7;

            int nBotMax = CrossManager.Instance.GetNumberPoint();
            if (nBot > nBotMax)
            {
                nBot = nBotMax;
            }

            for (int i = 0; i < nBot; i++)
            {
                BotRectiligne bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "BotRectiligne"),
                    Vector3.zero, Quaternion.identity).GetComponent<BotRectiligne>();
                Bots.Add(bot);
                
                bot.SetIndexPreviousPoint(i);
            }
        }
    }
}