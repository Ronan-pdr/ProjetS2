using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;

namespace Script.Bot
{
    public enum TypeBot
    {
        Rectiligne,
        Fuyard,
        Guide
    }
    
    public class BotManager : MonoBehaviour
    {
        // Chaque joueur va contrôler un certain nombre de bots,
        // les leurs seront stockés dans le dossier 'BotManager' sur Unity
        // et ceux controlés pas les autres dans 'DossierOtherBot'
        
        // c'est possible puisqu'il y en a qu'un par joueur
        public static BotManager Instance; 
    
        // stocker tous les bots
        private List<BotClass> Bots;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Bots = new List<BotClass>();

            // récupérer les type des bots ainsi que leur nombre
            TypeBot[] types = MasterManager.Instance.GetManagerGame().GetTypeBot();
            int nBot = types.Length;
            
            // trouver l'index du player du bot
            int indexPlayer = GetIndexPlayer(PhotonNetwork.LocalPlayer);

            for (int i = 0; i < nBot; i++) // Instancier, ranger (dans la liste) et positionner sur la map tous les bots
            {
                CreateBot(types[i], i + indexPlayer * nBot);
            }
        }

        private void CreateBot(TypeBot t, int indexSpot)
        {
            string type = GetStringBot(t);
            
            BotClass bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", type),
                Vector3.zero, Quaternion.identity).GetComponent<BotClass>();
            
            CrossPoint crossPoint = CrossManager.Instance.GetPoint(indexSpot); // récupérer son cross point
            bot.transform.position = crossPoint.transform.position; // le placer sur la map
            bot.SetOwnBotManager(this); // lui indiquer quel est son père (dans la hiérarchie de Unity)
          
            // les enregistrer dans une liste (cette liste contiendra seulement les bots que l'ordinateur contrôle)
            Bots.Add(bot);

            if (bot.GetComponent<BotRectiligne>())
            {
                ((BotRectiligne)bot).SetCrossPoint(crossPoint);
            }
        }

        private string GetStringBot(TypeBot type)
        {
            switch (type)
            {
                case TypeBot.Rectiligne:
                    return "BotRectiligne";
                case TypeBot.Fuyard:
                    return "Fuyard";
                case TypeBot.Guide:
                    return "Guide";
                default:
                    throw new Exception($"Le cas du {type} n'a pas encore été géré");
            }
        }

        private int GetIndexPlayer(Player player)
        {
            int i;
            Player[] players = PhotonNetwork.PlayerList;
            
            for (i = players.Length - 1; i >= 0 && !players[i].Equals(player); i--)
            {}

            return i;
        }

        // si la valeur de retour est le "Vector.zero", alors il n'y a pas de bon spot
        public Vector3 GetGoodSpot(BotClass fuyard, Vector3 posChasseur)
        {
            // trouvons le bot qui est le plus proche du fuyard tout en ayant une distace minimale
            // et en même temps plus loin pour le chasseur que pour le fuyard
            Vector3 posFuyard = fuyard.transform.position;

            Vector3 bestPos = Vector3.zero;
            float minDist = 200;
            foreach (BotClass bot in Bots)
            {
                if (bot == fuyard)
                    continue;

                Vector3 posBot = bot.transform.position;

                float distWithFuyard = Calcul.Distance(posFuyard, posBot);
                float distWithChasseur = Calcul.Distance(posChasseur, posBot);

                if (3 < distWithFuyard && distWithFuyard < minDist && distWithFuyard < distWithChasseur)
                {
                    minDist = distWithFuyard;
                    bestPos = posBot;
                }
            }

            if (SimpleMath.IsEncadré(bestPos, Vector3.zero)) // aucun bon spot
            {
                return Vector3.zero;
            }

            // y'a un bon spot et je vais répupérer la position
            // du cross point le plus proche
            CrossManager crossMan = CrossManager.Instance;
            
            Vector3 res = Vector3.zero;
            minDist = 100;
            int len = crossMan.GetNumberPoint();

            for (int i = 0; i < len; i++)
            {
                Vector3 posPoint = crossMan.GetPosition(i);
                float dist = Calcul.Distance(bestPos, posPoint);

                if (dist < minDist)
                {
                    res = posPoint;
                    minDist = dist;
                }
            }

            return res;
        }

        public void Die(GameObject bot)
        {
            Bots.Remove(bot.GetComponent<BotClass>()); // le supprimer de la liste
        
            PhotonNetwork.Destroy(bot.gameObject); // détruire l'objet
        }
    }
}
