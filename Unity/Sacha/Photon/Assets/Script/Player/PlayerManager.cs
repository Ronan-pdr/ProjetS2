using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using TMPro;
using Script;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    //public static PlayerManager Instance;
    
    private PhotonView PV;
    private GameObject controller;

    private List<GameObject> Bots;
    
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                controller = CreateController("Chasseur");
            }
            else
                controller = CreateController("Chassé");
        }
    }

    GameObject CreateController(string type) // Instanstiate our player controller
    {
        Transform tr = SpawnManager.Instance.GetSpawnPoint();
        return PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", type),
            tr.position, Quaternion.identity, 0, new object[]{PV.ViewID});
    }
    
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController("Chassé");
    }
}
