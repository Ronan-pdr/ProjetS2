using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

namespace Script.IA
{
    public class BotManager : MonoBehaviour
    {
        private List<BotClass> Bots;
        private Player[] players;

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Bots = new List<BotClass>();
            }
            
        }

        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                players = PhotonNetwork.PlayerList;
                int nBot = players.Length*10;
                            
                for (int i = 0; i < nBot; i++)
                {
                    BotClass bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "BotRectiligne"),
                        Vector3.zero, Quaternion.identity).GetComponent<BotClass>();
                    Bots.Add(bot);
                }
            }
            
        }
    }
}