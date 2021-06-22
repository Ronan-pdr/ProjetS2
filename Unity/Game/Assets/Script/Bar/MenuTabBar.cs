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
        [SerializeField] private PlayerListItem playerListItemPrefab;
        [SerializeField] Transform[] playerListContent;

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
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ChangeName(newPlayer);
            SetListPlayer();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            SetListPlayer();
        }

        // ------------ Private Methods ------------
        
        private void SetListPlayer()
        {
            Player[] players = PhotonNetwork.PlayerList;
            
            //Détruit tous les joueurs précédements enregistré dans la room
            foreach (Transform list in playerListContent)
            {
                foreach (Transform child in list)
                {
                    Destroy(child.gameObject);
                }
            }

            int l = players.Length;
            int m = playerListContent.Length;

            for (int i = 0; i < l; i++)
            {
                Instantiate(playerListItemPrefab, playerListContent[i % m]).GetComponent<PlayerListItem>().SetUp(players[i]);
            }
        }
        
        private void ChangeName(Player newPlayer)
        {
            string nickName = newPlayer.NickName;
            Player[] players = PhotonNetwork.PlayerListOthers;
            int count = 0;
            int l = players.Length;

            for ((int j, int i) = (0, 0); j < l - 1 && i != l; j++)
            {
                for (i = 0; i < l && !ChangedName(players[j]); i++)
                {}
            }

            bool ChangedName(Player player)
            {
                if (!player.Equals(newPlayer) && player.NickName == newPlayer.NickName)
                {
                    count += 1;
                    newPlayer.NickName = nickName + count;
                    return true;
                }

                return false;
            }
        }
    }
}