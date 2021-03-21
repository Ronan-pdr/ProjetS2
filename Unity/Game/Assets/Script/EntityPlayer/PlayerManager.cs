using UnityEngine;
using Photon.Pun;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Script.Bot;
using Script.DossierPoint;

namespace Script.EntityPlayer
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        // photon
        private PhotonView Pv;
    
        private void Awake()
        {
            transform.parent = MasterManager.Instance.transform;
        
            Pv = GetComponent<PhotonView>();
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
                
                string s = (string) value;
                int len = s.Length;

                // type du joueur
                MasterManager.TypePlayer typePlayer = (MasterManager.TypePlayer) int.Parse(((string) value).Substring(len - 1, 1));
                    
                // index du point que l'on retrouve dans le SpawnManager
                int indexSpot = int.Parse(s.Substring(0, 2));
                    
                // récupérer le transform du point en fonction du type du joueur (les chasseurs et les chassés n'ont pas les mêmes spawns)
                Transform trPoint;
                if (typePlayer == MasterManager.TypePlayer.Chasseur)
                {
                    trPoint = SpawnManager.Instance.GetTrSpawnPointChasseur(indexSpot);
                }
                else
                {
                    trPoint = SpawnManager.Instance.GetTrSpawnPointChassé(indexSpot);
                }
                    
                CreateController(typePlayer, trPoint);
            }
        }
    
        private void CreateController(MasterManager.TypePlayer type, Transform tr) // Instanstiate our player
        {
            string t = "";
            switch (type)
            {
                case MasterManager.TypePlayer.Chasseur:
                    t = "Chasseur";
                    break;
                case MasterManager.TypePlayer.Chassé:
                    t = "Chassé";
                    break;
                default:
                    Debug.Log($"Un script a tenté de créer un joueur de type {type}");
                    break;
            }

            GameObject controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", t),
                tr.position, tr.rotation, 0, new object[]{Pv.ViewID});
        
            MasterManager.Instance.SetOwnPlayer(controller.GetComponent<PlayerClass>()); // indiquer quel est ton propre joueur au MasterManager
        }
    }
}
