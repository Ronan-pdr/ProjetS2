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
        Suiveur,
        Hirondelle
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

        private MasterManager _masterManager;
        
        // ------------ Getter ------------
        public string GetNameBot(BotClass bot, Player player)
        {
            if (_masterManager.IsMultijoueur)
            {
                int i = ManList<Player>.GetIndex(PhotonNetwork.PlayerList, player);
            
                nBotNamed[i] += 1;
                return $"{player.NickName}{bot.GetTypeEntity()}{nBotNamed[i]}";
            }

            nBotNamed[0] += 1;
            return $"{bot.GetTypeEntity()}{nBotNamed[0]}";
        }

        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Bots = new List<BotClass>();
            _masterManager = MasterManager.Instance;

            if (_masterManager.IsMultijoueur)
            {
                nBotNamed = new int[PhotonNetwork.PlayerList.Length];
            }
            else
            {
                nBotNamed = new int[1];
            }
        }

        // ------------ Méthodes ------------
        public void CreateBot(TypeBot t, int indexSpawn)
        {
            (Transform tr, string type) = GetTrAndString(t, indexSpawn);

            BotClass bot;
            if (_masterManager.IsMultijoueur)
            {
                bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", type),
                    tr.position, tr.rotation).GetComponent<BotClass>();
            }
            else
            {
                bot = Instantiate(_masterManager.GetOriginalHirondelle(), tr.position, tr.rotation);
            }
            
            
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
                case TypeBot.Hirondelle:
                    return (tr, "Hirondelle");
                default:
                    throw new Exception($"Le cas du {t} n'a pas encore été géré");
            }
        }

        // si la valeur de retour est le "Vector.zero", alors il n'y a pas de bon spot
        public Vector3 GetGoodSpot(BotClass fuyard, Vector3 posChasseur)
        {
            // trouvons le bot qui est le plus loin du fuyard,
            // en étant à la même altitude que le fuyard et
            // le Fuyard doit être plus proche que le chasseur
            
            Vector3 posFuyard = fuyard.transform.position;

            Vector3 bestPosBot = Vector3.zero;
            float maxDist = 3;
            foreach (BotClass bot in Bots)
            {
                if (bot == fuyard)
                {
                    // il va pas fuir vers lui-même (logique hehe)
                    continue;
                }

                Vector3 posBot = bot.transform.position;
                
                if (!SimpleMath.IsEncadré(Calcul.Distance(posFuyard.y, posBot.y), 0.05f))
                {
                    // pas à la même altitude
                    continue;
                }

                float distDestWithFuyard = Calcul.Distance(posFuyard, posBot);
                float distDestWithChasseur = Calcul.Distance(posChasseur, posBot);
                float distFuyardWithChasseur = Calcul.Distance(posFuyard, posChasseur);
                    
                if (maxDist < distDestWithFuyard && distDestWithFuyard < distDestWithChasseur && distFuyardWithChasseur < distDestWithChasseur)
                {
                    maxDist = distDestWithFuyard;
                    bestPosBot = posBot;
                }
            }

            if (SimpleMath.IsEncadré(bestPosBot, Vector3.zero)) // aucun bon spot
            {
                return Vector3.zero;
            }

            return bestPosBot;
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
