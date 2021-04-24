using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Bot;
using Script.InterfaceInGame;
using Script.Manager;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Script.EntityPlayer
{
    public abstract class PlayerClass : Humanoide
    {
        // ------------ SerializeField ------------
        
        [Header("Camera")]
        [SerializeField] protected Transform cameraHolder;
        
        // ------------ Etat ------------
        protected enum Etat
        {
            Debout,
            Assis,
            Accroupi
        }

        protected Etat etat = Etat.Debout;
        protected float LastChangementEtat; // La dernière fois qu'on a changé de position entre assis et lever
        
        // ------------ Attributs ------------
        
        private TouchesClass touches;

        //Look
        private float verticalLookRotation; 
        private float mouseSensitivity = 3f;

        //Rassembler les infos
        protected Transform masterManager;
        
        // ------------ Constructeurs ------------
        
        protected abstract void AwakePlayer();
        private void Awake()
        {
            AwakeHuman();
            AwakePlayer();
        
            // parenter
            masterManager = MasterManager.Instance.transform;
            Tr.parent = masterManager;

            if (Pv.IsMine)
            {
                // indiquer quel est ton propre joueur au MasterManager
                MasterManager.Instance.SetOwnPlayer(this);
            }
        
            name = Pv.Owner.NickName;
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutPlayer(this);
        }
        protected abstract void StartPlayer();
        private void Start()
        {
            StartPlayer();
            StartHuman();
            touches = TouchesClass.Instance;
        
            if (!Pv.IsMine) 
            {
                // On veut détruire les caméras qui ne sont pas les tiennes
                Destroy(GetComponentInChildren<Camera>().gameObject);
            }
        }

        // ------------ Update ------------
        protected abstract void UpdatePlayer();

        private void Update()
        {
            // tout les ordis doivent le faire

            if (!Pv.IsMine || master.IsGameEnded())
                return;
            
            PotentielleMort();
            
            if (IsPause())
            {
                MoveAmount = Vector3.zero;
                return;
            }
            
            UpdatePlayer();
            
            Look();
            Move();
            
            if (Input.GetKey(touches.GetKey(TypeTouche.Jump)) && etat == Etat.Debout)
            {
                Jump();
            }

            UpdateHumanoide();
        }

        private void FixedUpdate()
        {
            if (!Pv.IsMine)
                return;
        
            MoveEntity();
        }
    
        // ------------ Méthodes ------------
        private void Move()
        {
            if (etat == Etat.Assis) // il se déplace pas quand il est assis
                return;
        
            int zMov = 0;
            if (Input.GetKey(touches.GetKey(TypeTouche.Avancer)))
                zMov++;
            if (Input.GetKey(touches.GetKey(TypeTouche.Reculer)))
                zMov--;
            int xMov = 0;
            if (Input.GetKey(touches.GetKey(TypeTouche.Droite)))
                xMov++;
            if (Input.GetKey(touches.GetKey(TypeTouche.Gauche)))
                xMov--;

            float speed = WalkSpeed;
            if (zMov == 1 && xMov == 0 && Input.GetKey(touches.GetKey(TypeTouche.Sprint))) // il faut qu'il avance tout droit pour sprinter
                speed = SprintSpeed;
            else if (xMov != 0 || zMov != 0) // en gros, s'il se déplace
            {
                etat = Etat.Debout; // il ne reste pas accroupi lorqu'il se déplace pas tout droit
            }

            Vector3 moveDir = new Vector3(xMov, 0, zMov);

            SetMoveAmount(moveDir, speed);
        }

        private void Look()
        {
            Tr.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

            verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -70f, 30f);

            cameraHolder.localEulerAngles = Vector3.left * verticalLookRotation;
        }

        public static bool IsPause()
        {
            if (PauseMenu.Instance.GetIsPaused())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return true;
            }
            
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            return false;
        }

        private void OnDestroy()
        {
            MasterManager.Instance.Die(this);
        }

        protected override void Die()
        {
            enabled = false;
            anim.enabled = false;

            // On ne détruit pas le corps des autres joueurs
            if (Pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        
        // ------------ Photon ------------
    
        // Communication par hash
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!Pv.Owner.Equals(targetPlayer)) // si c'est pas toi la target, tu ne changes rien
                return;
        
            // arme du chasseur -> EquipItem (Chasseur)
            if (this is Chasseur && !Pv.IsMine) // ça ne doit pas être ton point de vie puisque tu l'as déjà fait
            {
                if (changedProps.TryGetValue("itemIndex", out object indexArme))
                {
                    Chasseur chasseur = (Chasseur) this;
                    chasseur.EquipItem((int)indexArme);
                }
            }

            // point de vie -> TakeDamage (Humanoide)
            // Tout le monde doit faire ce changement (trop compliqué de retrouvé celui qui l'a déjà fait)
            if (changedProps.TryGetValue("PointDeViePlayer", out object vie))
            {
                CurrentHealth = (int)vie;
            }
        }
        
        // ------------ Animation ------------
        protected void AnimationTernier()
        {
            (int xMov, int zMov) = (0, 0);
            if (Input.GetKey(touches.GetKey(TypeTouche.Avancer))) // avancer
                zMov += 1;
            if (Input.GetKey(touches.GetKey(TypeTouche.Reculer))) // reculer
                zMov -= 1;
            if (Input.GetKey(touches.GetKey(TypeTouche.Droite))) // droite
                xMov += 1;
            if (Input.GetKey(touches.GetKey(TypeTouche.Gauche))) // gauche
                xMov -= 1;


            if (Input.GetKey(touches.GetKey(TypeTouche.Assoir)) && etat != Etat.Accroupi && Time.time - LastChangementEtat > 0.5f) // il ne doit pas être accroupi
            {
                if (etat == Etat.Debout) // S'assoir puisqu'il est debout
                {
                    MoveAmount = Vector3.zero;
                    ActiverAnimation("Assis");
                    etat = Etat.Assis;
                }
                else // Se lever depuis assis
                {
                    ActiverAnimation("Lever PASS");
                    etat = Etat.Debout;
                }

                LastChangementEtat = Time.time;
            }
            else if (Input.GetKey(touches.GetKey(TypeTouche.Accroupi)) && etat != Etat.Assis && Time.time - LastChangementEtat > 0.5f) // il ne doit pas être assis
            {
                if (etat == Etat.Debout) // s'accroupir puisqu'il est debout
                {
                    MoveAmount = Vector3.zero;
                    ActiverAnimation("Accroupir");
                    etat = Etat.Accroupi;
                }
                else // se lever depuis accroupi
                {
                    ActiverAnimation("Lever PAcc");
                    etat = Etat.Debout;
                }
                
                LastChangementEtat = Time.time;
            }
            else if (etat == Etat.Assis || Time.time - LastChangementEtat < 0.25f) // Aucune animation lorsque le chassé est assis et s'assois/s'accroupi
            {}
            else if (zMov == 1) // Avancer
            {
                if (etat == Etat.Accroupi) // avancer en étant accroupi
                {
                    ActiverAnimation("Marche acc");
                }
                else if (xMov == 0 && Input.GetKey(touches.GetKey(TypeTouche.Sprint))) // Sprinter
                {
                    ActiverAnimation("Course");
                }
                else // Avancer normalement
                {
                    ActiverAnimation("Avant");
                }
            }
            else if (zMov == -1) // Reculer
            {
                ActiverAnimation("Arriere");
            }
            else if (xMov == 1) // Droite
            {
                ActiverAnimation("Droite");
            }
            else if (xMov == -1) // Gauche
            {
                ActiverAnimation("Gauche");
            }
            else if (Input.GetKey(touches.GetKey(TypeTouche.Jump))) // Jump
            {
                ActiverAnimation("Jump");
            }
            else
            {
                AnimationStop();
            }
        }
    }
}
