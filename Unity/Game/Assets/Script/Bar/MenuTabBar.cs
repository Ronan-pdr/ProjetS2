using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Bar
{
    public class MenuTabBar : MonoBehaviourPunCallbacks
    {
        // ------------ SerializeField ------------
        
        [Header("Pour liste joueurs")]
        [SerializeField] TextMeshProUGUI[] playerListContent;

        [Header("Room")]
        [SerializeField] private TMP_Text roomNameText;

        // ------------ Constructeur ------------

        private void Awake()
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            SetListPlayer();
        }
        
        // ------------ Public Methods ------------

        public void Open()
        {
            MenuManager.Instance.OpenMenu(GetComponent<Menu.Menu>());
        }

        // ------------ Private Methods ------------
        
        private void SetListPlayer()
        {
            // effacer
            foreach (TextMeshProUGUI content in playerListContent)
            {
                content.text = "";
            }
            
            // récup info
            Player[] players = PhotonNetwork.PlayerList;
            int nbPlayer = players.Length;
            int nContent = playerListContent.Length;
            
            // écrire
            for (int i = 0; i < nbPlayer; i++)
            {
                playerListContent[i % nContent].text += players[i].NickName + Environment.NewLine;
            }
        }

        private string[] RecupNameOtherPlayers()
        {
            Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
            int l = otherPlayers.Length;

            string[] namesOtherPlayer = new string[l];

            for (int i = 0; i < l; i++)
            {
                namesOtherPlayer[i] = otherPlayers[i].NickName;
            }

            return namesOtherPlayer;
        }
        
        // ------------ Event ------------
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            newPlayer.NickName = ChangeName(newPlayer.NickName, RecupNameOtherPlayers());
            SetListPlayer();
        }

        public override void OnPlayerLeftRoom(Player _)
        {
            SetListPlayer();
        }
        
        // ------------ Static Methods ------------
        
        public static string ChangeName(string namePlayer, string[] namesOtherPlayer)
        {
            string res = namePlayer;
            string[] players = namesOtherPlayer;
            int count = 1;
            int l = players.Length;
            
            // arthur2 ; arthur

            for ((int j, int i) = (0, 0); j < l && i != l; j++)
            {
                for (i = 0; i < l && !ChangedName(namesOtherPlayer[i]); i++)
                {}
            }

            bool ChangedName(string nameOtherPlayer)
            {
                if (nameOtherPlayer == res)
                {
                    count += 1;
                    res = namePlayer + count;
                    return true;
                }

                return false;
            }

            return res;
        }
    }
}