using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;

namespace Script.Bot
{
    // ------------ Type ------------
    public enum TypeBot
    {
        Rectiligne,
        Fuyard,
        Guide,
        Suiveur
    }
    
    public class BotManager : MonoBehaviourPunCallbacks
    {
        // Chaque joueur va contrôler un certain nombre de bots,
        // les leurs seront stockés dans le dossier 'BotManager' sur Unity
        // et ceux controlés pas les autres dans 'DossierOtherBot'
        
        // ------------ Attributs ------------
        
        // c'est possible puisqu'il y en a qu'un par joueur
        public static BotManager Instance; 
    
        // stocker tous les bots
        private List<BotClass> Bots;
        
        // cette liste va servir à donner les noms à chaque bot
        private int[] nBotNamed;
        
        // ------------ Getter ------------
        public string GetNameBot(BotClass bot, Player player)
        {
            int i = ManList<Player>.GetIndex(PhotonNetwork.PlayerList, player);
            
            nBotNamed[i] += 1;
            return $"{player.NickName}{bot.GetTypeEntity()}{nBotNamed[i]}";
        }

        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Bots = new List<BotClass>();
            nBotNamed = new int[PhotonNetwork.PlayerList.Length];
        }

        // ------------ Méthodes ------------
        private void CreateBot(TypeBot t, int indexSpawn)
        {
            (Transform tr, string type) = GetTrAndString(t, indexSpawn);

            BotClass bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", type),
                tr.position, tr.rotation).GetComponent<BotClass>();
            
            if (bot is BotRectiligne)
            {
                ((BotRectiligne)bot).SetCrossPoint(CrossManager.Instance.GetPoint(indexSpawn));
            }

            bot.SetOwnBotManager(this); // lui indiquer quel est son père (dans la hiérarchie de Unity)
          
            // les enregistrer dans une liste (cette liste contiendra seulement les bots que l'ordinateur contrôle)
            Bots.Add(bot);
        }

        private (Transform, string) GetTrAndString(TypeBot t, int indexSpawn)
        {
            Transform tr;
            if (t == TypeBot.Rectiligne)
            {
                string type = "BotRectiligne";
                tr = CrossManager.Instance.GetPoint(indexSpawn).transform;
                
                // pas de rotation initiale avec les crossPoints
                tr.transform.rotation = Quaternion.identity;

                return (tr, type);
            }
            
            tr = SpawnManager.Instance.GetTrBot(indexSpawn);
            switch (t)
            {
                case TypeBot.Fuyard:
                    return (tr, "Fuyard");
                case TypeBot.Guide:
                    return (tr, "Guide");
                case TypeBot.Suiveur:
                    return (tr, "Suiveur");
                default:
                    throw new Exception($"Le cas du {t} n'a pas encore été géré");
            }
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

        public void Die(BotClass bot)
        {
            // seul le créateur détruit son bot
            if (bot.IsMyBot())
            {
                // le supprimer de la liste
                Bots.Remove(bot);
            }
        }
        
        // ------------ Multijoueur ------------
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            // Si c'est pas toi la target, tu ne créés pas les bots
            if (!PhotonNetwork.LocalPlayer.Equals(targetPlayer))
                return;
            
            // bien vérifier que le changement a été fait
            if (!changedProps.TryGetValue("InfoCréationBots", out object value))
                return;

            foreach ((int indexSpot, TypeBot typeBot) in DecodeFormatInfoBot((string) value))
            {
                CreateBot(typeBot, indexSpot);
            }
        }
        
        // Exemple -> "1 2;3 2;15 0"
        public static string EncodeFormatInfoBot((int indexSpot, TypeBot type)[] arr)
        {
            int l = arr.Length;
            if (l == 0)
                return "";
            
            string res = Aux(arr[0].indexSpot, arr[0].type);
            
            for (int i = 1; i < l; i++)
            {
                (int indexSpot, TypeBot type) = arr[i];
                res += ";" + Aux(indexSpot, type);
            }

            return res;

            string Aux(int indexSpot, TypeBot type) => $"{indexSpot} {(int)type}";
        }

        private static (int indexSpot, TypeBot typeBot)[] DecodeFormatInfoBot(string s)
        {
            string[] listInfos = s.Split(';');
            int l = listInfos.Length;
            
            (int, TypeBot)[] res = new (int, TypeBot)[l];

            for (int i = 0; i < l; i++)
            {
                string[] infos = listInfos[i].Split(' ');
                
                // type du bot
                TypeBot typeBot = (TypeBot) int.Parse(infos[1]);
                    
                // index du point que l'on retrouve dans le SpawnManager
                int indexSpot = int.Parse(infos[0]);

                res[i] = (indexSpot, typeBot);
            }
            
            return res;
        }
    }
}
