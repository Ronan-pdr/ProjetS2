using UnityEngine;
using Photon.Pun;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
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
                changedProps.TryGetValue("typePlayer", out object value);
           
                if (value != null) // bien vérifier que le changement a été fait sur le type du joueur
                {
                    string typePlayer = (string) value;
                    CreateController(typePlayer);
                } 
            }
        }
    
        private void CreateController(string type) // Instanstiate our player
        {
            if (type != "Chasseur" && type != "Chassé")
            {
                Debug.Log($"Un script a tenté de créer un joueur de type {type}");
            }

            Transform tr = SpawnManager.Instance.GetSpawnPoint();
            GameObject controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", type),
                tr.position, tr.rotation, 0, new object[]{Pv.ViewID});
        
            MasterManager.Instance.SetOwnPlayer(controller.GetComponent<PlayerClass>()); // indiquer quel est ton propre joueur au MasterManager
        }
    }
}

