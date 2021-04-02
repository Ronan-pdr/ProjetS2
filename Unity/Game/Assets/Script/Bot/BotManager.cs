using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Script.DossierPoint;
using Script.EntityPlayer;

namespace Script.Bot
{
    public class BotManager : MonoBehaviour
    {
        // Chaque joueur va contrôler un certain nombre de bots,
        // les leurs seront stockés dans le dossier 'BotManager' sur Unity
        // et ceux controlés pas les autres dans 'DossierOtherBot'
    
    
        public static BotManager Instance; //c'est possible puisqu'il y en a qu'un par joueur
    
        // stocké tous les bots
        private List<BotClass> Bots;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            // pas de création de bot s'il y a maintenance
            if (MasterManager.Instance.IsInMaintenance())
                return;
            
            Bots = new List<BotClass>();

            int nBot = 1;
            int indexPlayer;
            Player[] players = PhotonNetwork.PlayerList;
            for (indexPlayer = players.Length - 1; indexPlayer >= 0 && !players[indexPlayer].Equals(PhotonNetwork.LocalPlayer); indexPlayer--)
            {} // trouver l'index du player du bot

            for (int i = 0; i < nBot; i++) // Instancier, ranger (dans la liste) et positionner sur la map tous les bots
            {
                BotClass bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "BotRectiligne"),
                    Vector3.zero, Quaternion.identity).GetComponent<BotClass>();

                CrossPoint crossPoint = CrossManager.Instance.GetPoint(i + indexPlayer * nBot);
                
                bot.transform.position = crossPoint.transform.position; // le placer sur la map
                
                bot.SetBot(crossPoint);

                bot.SetOwnBotManager(this); // lui indiquer quel est son père
            
                Bots.Add(bot); // les enregistrer dans une liste (cette liste contiendra seulement les bots que l'ordinateur contrôle)
            }
        }

        public void Die(GameObject bot)
        {
            Bots.Remove(bot.GetComponent<BotClass>()); // le supprimer de la liste
        
            PhotonNetwork.Destroy(bot.gameObject); // détruire l'objet
        }
    }
}
