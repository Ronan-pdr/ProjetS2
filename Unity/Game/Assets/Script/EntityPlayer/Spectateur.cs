using Photon.Pun;
using UnityEngine;
using Script.InterfaceInGame;
using Script.Manager;
using Script.Tools;

namespace Script.EntityPlayer
{
    public class Spectateur : Entity
    {
        // ------------ Attributs ------------
        
        // Celui que l'on va suivre
        protected Transform Porteur;
        private int indexPorteur;
        
        //Photon
        protected PhotonView Pv;

        //Rassembler les infos
        private Transform masterManager;
        
        //Variable similaire aux playerClass
        private float yLookRotation;
        private float xLookRotation;
        private float mouseSensitivity = 3f;

        // hauteur pour atteindre la tête
        private float hauteur = 1.4f;
        
        // ------------ Setter ------------
        private void SetPorteur()
        {
            Porteur = MasterManager.Instance.GetPlayer(indexPorteur).transform;
        }
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            // Le ranger dans MasterClient
            masterManager = MasterManager.Instance.transform;
            transform.parent = masterManager;
        
            SetRbTr();
            Pv = GetComponent<PhotonView>();

            indexPorteur = 0;
            SetPorteur();
        }

        private void Start()
        {
            if (Pv.IsMine)
            {
                Tr.rotation = Porteur.rotation;
            }
            else
            {
                Destroy(GetComponentInChildren<Camera>().gameObject); // On veut détruire les caméras qui ne sont pas les tiennes
            }
        }

        // ------------ Update ------------
        private void Update()
        {
            if (!Pv.IsMine)
                return;

            if (PlayerClass.IsPause())
                return;

            // le cas ou l'ancier porteur est mort ou à quitter la partie
            if (!Porteur)
            {
                indexPorteur = 0;
                SetPorteur();
            }

            Position();
            Look();
            ChangerPorteur();
        }

        // ------------ Méthodes ------------
        
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
            
            SetPorteur();
        }
    }
}