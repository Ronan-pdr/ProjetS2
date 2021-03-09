using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Script.DossierArme;
using Script.InterfaceInGame;
using Script.Tools;

namespace Script.EntityPlayer
{
    public class Chasseur : PlayerClass
    {
        // Relatif aux armes
        [SerializeField] private Arme[] armes;
        private int armeIndex;
        private int previousArmeIndex = -1;
    
        //Getter
        private void Awake()
        {
            AwakePlayer();
        
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutChasseur(this);
        }

        void Start()
        {
            MaxHealth = 100;
            StartPlayer();
        
            EquipItem(0);
        }
        
        void Update()
        {
            if (!Pv.IsMine)
                return;
        
            Cursor.lockState = PauseMenu.Instance.GetIsPaused() ? CursorLockMode.None : CursorLockMode.Confined;
            Cursor.visible = PauseMenu.Instance.GetIsPaused();
        
            if (PauseMenu.Instance.GetIsPaused())
            {
                MoveAmount = Vector3.zero;
                return;
            }

            ManipulerArme();
            
            UpdatePlayer();
        }

        private void FixedUpdate()
        {
            FixedUpdatePlayer();
        }
    

        //GamePlay
    
        private void ManipulerArme()
        {
            //changer d'arme avec les numéros
            for (int i = 0; i < armes.Length; i++)
            {
                if (Input.GetKey((i + 1).ToString()))
                {
                    EquipItem(i);
                    break;
                }
            }

            //changer d'arme avec la molette
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                EquipItem(SimpleMath.Mod(previousArmeIndex + 1, armes.Length));
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                EquipItem(SimpleMath.Mod(previousArmeIndex - 1, armes.Length));
            }

            //tirer
            if (Input.GetMouseButton(0))
            { 
                armes[armeIndex].Use();
            }
        }
        
        public void EquipItem(int index) // index supposé valide
        {
            // Le cas où on essaye de prendre l'arme qu'on a déjà
            if (index == previousArmeIndex)
                return;
            
            // C'est le cas où on avait déjà une arme, il faut la désactiver
            if (previousArmeIndex != -1)
            {
                armes[previousArmeIndex].armeObject.SetActive(false);
            }

            previousArmeIndex = armeIndex;
            
            armeIndex = index;
            armes[armeIndex].armeObject.SetActive(true);

            if (Pv.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("itemIndex", armeIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        // GamePlay

        protected override void Die()
        {
        
        }
    }
}
