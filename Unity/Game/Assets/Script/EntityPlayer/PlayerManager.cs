using UnityEngine;
using Photon.Pun;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Script;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    // photon
    private PhotonView PV;
    
    // joueur
    private GameObject controller;
    
    private void Awake()
    {
        transform.parent = MasterManager.Instance.transform;
        
        PV = GetComponent<PhotonView>();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.Owner.Equals(targetPlayer)) // si c'est pas toi la target, tu ne changes rien
            return;

        // type du joueur pour qu'il se fasse instancier pour la première fois -> Update (MasterManager)
        if (PV.IsMine)
        {
            changedProps.TryGetValue("typePlayer", out object _typePlayer);
           
            if (_typePlayer != null) // bien vérifier que le changement a été fait sur le type du joueur
            {
                string typePlayer = (string) _typePlayer;
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
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", type),
            tr.position, tr.rotation, 0, new object[]{PV.ViewID});
        
        MasterManager.Instance.SetOwnPlayer(controller.GetComponent<PlayerClass>()); // indiquer quel est ton propre joueur au MasterManager
    }
}
