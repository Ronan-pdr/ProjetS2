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
        private Transform _porteur;
        private int indexPorteur;
        
        //Photon
        protected PhotonView Pv;
        
        //Variable similaire aux playerClass
        private float yLookRotation;
        private float xLookRotation;
        private float mouseSensitivity = 3f;

        // hauteur pour atteindre la tête
        private float hauteur = 1.4f;
        
        // interface
        private InterfaceInGameManager _interfaceInGameManager;
        
        // ------------ Setter ------------
        private void SetPorteur()
        {
            _porteur = master.GetPlayer(indexPorteur).transform;
            Position();

            if (Pv.IsMine)
            {
                _interfaceInGameManager.SetNameForSpect(_porteur.name);
            }
        }
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            // primordial
            SetRbTr();

            // Le ranger dans MasterClient
            transform.parent = master.transform;
            
            // interface
            _interfaceInGameManager = InterfaceInGameManager.Instance;
            _interfaceInGameManager.NewSpect();

            if (Pv.IsMine)
            {
                _interfaceInGameManager.ActiveNbSpect();
            }
            
            // reste
            Pv = GetComponent<PhotonView>();

            indexPorteur = 0;
            SetPorteur();
        }

        private void Start()
        {
            if (Pv.IsMine)
            {
                Tr.rotation = _porteur.rotation;
            }
            else
            {
                // On veut détruire les caméras qui ne sont pas les tiennes
                Destroy(GetComponentInChildren<Camera>().gameObject);
            }
        }

        // ------------ Update ------------
        private void Update()
        {
            if (!Pv.IsMine || master.IsGameEnded())
                return;
            
            // le cas ou l'ancier porteur est mort ou à quitter la partie
            if (!_porteur)
            {
                indexPorteur = 0;
                SetPorteur();
            }
            
            Position();

            if (PlayerClass.IsPause())
                return;

            Look();
            ChangerPorteur();
        }

        // ------------ Méthodes ------------
        
        private void Position()
        {
            Tr.position = _porteur.position + Vector3.up * hauteur;
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
                indexPorteur = SimpleMath.Mod(indexPorteur + 1, master.GetNbPlayer());
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                indexPorteur = SimpleMath.Mod(indexPorteur - 1, master.GetNbPlayer());
            }
            
            SetPorteur();
        }
    }
}