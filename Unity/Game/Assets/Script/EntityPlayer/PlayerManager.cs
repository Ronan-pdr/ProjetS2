using UnityEngine;
using Photon.Pun;
using System.IO;
using Script;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    private GameObject controller;

    private Transform masterManager;
    
    private void Awake()
    {
        masterManager = MasterManager.Instance.transform;
        transform.parent = masterManager;
        
        PV = GetComponent<PhotonView>();
        
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                controller = CreateController("Chasseur");
            }
            else
                controller = CreateController("Chassé");
            
            MasterManager.Instance.SetOwnPlayer(controller.GetComponent<PlayerClass>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    GameObject CreateController(string type) // Instanstiate our player
    {
        Transform tr = SpawnManager.Instance.GetSpawnPoint();
        return PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", type),
            tr.position, tr.rotation, 0, new object[]{PV.ViewID});
    }
}
