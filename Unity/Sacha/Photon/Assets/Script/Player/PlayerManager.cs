using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Script;

public class PlayerManager : MonoBehaviour
{
    //public static PlayerManager Instance;
    
    private PhotonView PV;
    private Chasseur chasseur;
    private Chassé[] listChassé;

    private GameObject controller;

    private void Awake()
    {
        /*if (Instance) // checks if another PlayerManager exists
        {
            Destroy(gameObject); // there can only be one
            return;
        }
        DontDestroyOnLoad(gameObject); // I am the only one...
        
        Instance = this;*/
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
        //listChassé = GetComponentsInChildren<Chassé>();
    }

    void CreateController() // Instanstiate our player controller
    {
        Transform tr = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Chasseur"),
            tr.position, tr.rotation, 0, new object[]{PV.ViewID});
    }

    private void Update()
    {
        /*for (int i = 0; i < listChassé.Length; i++)
        {
            Debug.Log(i);
        }*/
    }
    
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
