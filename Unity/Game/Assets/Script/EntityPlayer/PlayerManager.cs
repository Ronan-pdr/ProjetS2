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
        // photon
        private PhotonView Pv;
    
        private void Awake()
        {
            transform.parent = MasterManager.Instance.transform;
        
            Pv = GetComponent<PhotonView>();
        }
        
        private void CreateController(TypePlayer type, Transform tr) // Instanstiate our player
        {
            string t = "";
            switch (type)
            {
                case TypePlayer.Chasseur:
                    t = "Chasseur";
                    break;
                case TypePlayer.Chassé:
                    t = "Chassé";
                    break;
                case TypePlayer.Blocard:
                    t = "Blocard";
                    break;
                default:
                    Debug.Log($"Un script a tenté de créer un joueur de type {type}");
                    break;
            }

            GameObject controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", t),
                tr.position, tr.rotation, 0, new object[]{Pv.ViewID});
        
            // indiquer quel est ton propre joueur au MasterManager
            MasterManager.Instance.SetOwnPlayer(controller.GetComponent<PlayerClass>());
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

                (int indexSpot, TypePlayer typePlayer) = DecodeFormatInfoJoueur((string) value);

                // récupérer le transform du point en fonction du type du joueur (les chasseurs et les chassés n'ont pas les mêmes spawns)
                Transform trPoint;
                switch (typePlayer)
                {
                    case TypePlayer.Chasseur:
                        trPoint = SpawnManager.Instance.GetTrChasseur(indexSpot);
                        break;
                    case TypePlayer.Chassé:
                        trPoint = SpawnManager.Instance.GetTrChassé(indexSpot);
                        break;
                    case TypePlayer.Blocard:
                        trPoint = SpawnManager.Instance.GetTrChassé(indexSpot);
                        break;
                    default:
                        throw new Exception($"Le type {typePlayer} n'est pas encore géré");
                }
                    
                CreateController(typePlayer, trPoint);
            }
        }
    }
}
