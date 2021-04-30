using System;
using UnityEngine;
using Photon.Pun;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Script.Bot;
using Script.DossierPoint;
using Script.Manager;
using Script.Tools;

namespace Script.EntityPlayer
{
    public enum TypePlayer
    {
        Chasseur = 0,
        Chassé = 1,
        Blocard = 2,
        Spectateur = 3,
        None = 4,
        Player = 5
    }
    
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        // ------------ Attribut ------------

        public static PlayerManager Own;
        
        private PhotonView Pv;

        private bool _isQuitting;
        
        // Pour savoir ce que tu étais au début
        private TypePlayer _type;
        
        // ------------ Getter ------------

        public TypePlayer Type => _type;

        public bool IsQuitting => _isQuitting;
        
        // ------------ Setter ------------

        public void BeginToQuit() => _isQuitting = true;
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            transform.parent = MasterManager.Instance.transform;
            Pv = GetComponent<PhotonView>();

            if (Pv.IsMine)
            {
                Own = this;
            }
        }

        // ------------ Méthodes ------------
        private void CreateController(int indexSpawn) // Instanstiate our player
        {
            string t;
            Transform tr;
            switch (_type)
            {
                case TypePlayer.Chasseur:
                    t = "Chasseur";
                    tr = SpawnManager.Instance.GetTrChasseur(indexSpawn);
                    break;
                case TypePlayer.Chassé:
                    t = "Chassé";
                    tr = SpawnManager.Instance.GetTrChassé(indexSpawn);
                    break;
                case TypePlayer.Blocard:
                    t = "Blocard";
                    tr = SpawnManager.Instance.GetTrChassé(indexSpawn);
                    break;
                default:
                    throw new Exception("Un script a tenté de créer un joueur de type {type}");
            }

            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", t),
                tr.position, tr.rotation, 0, new object[]{Pv.ViewID});
        }

        // la string contenant les infos du joueur seront sous la forme :
        // indexCoordPoint(2 caractères) + type(1 caractère)
        public static string EncodeFormatInfoJoueur(int indexSpot, TypePlayer type)
        {
            return ManString.Format(indexSpot.ToString(), 2) + (int)type;
        }

        private static (int indexSpot, TypePlayer typePlayer) DecodeFormatInfoJoueur(string s)
        {
            int len = s.Length;

            // type du joueur
            TypePlayer typePlayer = (TypePlayer) int.Parse(s.Substring(len - 1, 1));
                    
            // index du point que l'on retrouve dans le SpawnManager
            int indexSpot = int.Parse(s.Substring(0, 2));

            return (indexSpot, typePlayer);
        }

        // ------------ Multijoueur ------------
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!Pv.Owner.Equals(targetPlayer)) // si c'est pas toi la target, tu ne changes rien
                return;

            // type du joueur pour qu'il se fasse instancier pour la première fois -> Update (MasterManager)
            if (Pv.IsMine)
            {
                changedProps.TryGetValue("InfoCréationJoueur", out object value);

                if (value == null) // bien vérifier que le changement a été fait
                    return;

                (int indexSpawn, TypePlayer typePlayer) = DecodeFormatInfoJoueur((string) value);

                _type = typePlayer;
                CreateController(indexSpawn);
            }
        }
    }
}
