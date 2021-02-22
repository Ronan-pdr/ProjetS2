using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV;
    private GameObject controller;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController() // Instanstiate our player controller
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Chassé"),
            new Vector3(0, 5, 0), Quaternion.identity, 0, new object[]{PV.ViewID});
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
