using System;
using Photon.Pun;
using UnityEngine;

namespace Script
{
    public class Spectateur : Entity
    {
        // Celui que l'on va suivre
        private Transform Porteur;
        private int indexPorteur;
        
        //Photon
        protected PhotonView PV;

        //Rassembler les infos
        protected Transform masterManager;
        
        //Variable similaire aux playerClass
        private float yLookRotation;
        private float xLookRotation;
        private float mouseSensitivity = 3f;

        [SerializeField] private float hauteur;
        
        private void Awake()
        {
            // Le ranger dans MasterClient
            masterManager = MasterManager.Instance.transform;
            transform.parent = masterManager;
        
            SetRbTr();
            PV = GetComponent<PhotonView>();

            indexPorteur = 0;
            Porteur = MasterManager.Instance.GetPlayer(indexPorteur).transform;
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                Tr.rotation = Porteur.rotation;
            }
            else
            {
                Destroy(GetComponentInChildren<Camera>().gameObject); // On veut détruire les caméras qui ne sont pas les tiennes
            }
        }

        private void Update()
        {
            if (!PV.IsMine || PauseMenu.PauseMenu.isPaused)
            {
                return;
            }

            Position();
            Look();
            ChangerPorteur();
        }

        private void Position()
        {
            Tr.position = Porteur.position + Vector3.up * hauteur;
        }

        private void Look()
        {
            xLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            xLookRotation = Mathf.Clamp(xLookRotation, -50f, 30f);
            
            yLookRotation += Input.GetAxisRaw("Mouse X") * mouseSensitivity;

            Tr.localEulerAngles = new Vector3(-xLookRotation, yLookRotation, 0);
        }

        private void ChangerPorteur()
        {
            //changer d'arme avec la molette
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                indexPorteur = SimpleMath.Mod(indexPorteur + 1, MasterManager.Instance.GetNbPlayer());
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                indexPorteur = SimpleMath.Mod(indexPorteur - 1, MasterManager.Instance.GetNbPlayer());
            }
            
            Porteur = MasterManager.Instance.GetPlayer(indexPorteur).transform;
        }
    }
}