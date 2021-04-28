using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Animation;
using Script.Animation.Personnages.Hunted;
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
            if (!this)
                return;

            UpdateMasterOfTheMaster();
            
            if (!Pv.IsMine || master.IsGameEnded())
                return;

            if (IsPause())
            {
                MoveAmount = Vector3.zero;
                return;
            }
            
            UpdatePlayer();
            
            Look();
            Move();
            
            if (touches.GetKey(TypeTouche.Jump) && etat == Etat.Debout)
            {
                Jump();
            }

            UpdateHumanoide();
            AnimationPlayer();
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
            if (touches.GetKey(TypeTouche.Avancer))
                zMov++;
            if (touches.GetKey(TypeTouche.Reculer))
                zMov--;
            int xMov = 0;
            if (touches.GetKey(TypeTouche.Droite))
                xMov++;
            if (touches.GetKey(TypeTouche.Gauche))
                xMov--;

            float speed;
            if (etat == Etat.Accroupi) // avance en étant accroupi
            {
                speed = SquatSpeed;
            }
            else if (zMov == 1 && xMov == 0 && touches.GetKey(TypeTouche.Sprint)) // il faut qu'il avance tout droit pour sprinter
            {
                speed = SprintSpeed;
            }
            else // avance normalement
            {
                speed = WalkSpeed;
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
            Anim.StopContinue();

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
        protected void AnimationPlayer()
        {
            (int xMov, int zMov) = (0, 0);
            if (touches.GetKey(TypeTouche.Avancer)) // avancer
                zMov += 1;
            if (touches.GetKey(TypeTouche.Reculer)) // reculer
                zMov -= 1;
            if (touches.GetKey(TypeTouche.Droite)) // droite
                xMov += 1;
            if (touches.GetKey(TypeTouche.Gauche)) // gauche
                xMov -= 1;


            if (!Grounded) // Jump
            {
                // S'il n'est pas au sol, il ne fait aucune animation
                // l'animation du jump est activé dans 'SetGroud' (Humanoide)
            }
            else if (!(this is Chasseur) && touches.GetKey(TypeTouche.Assoir) && etat != Etat.Accroupi && Time.time - LastChangementEtat > 0.5f) // il ne doit pas être accroupi
            {
                if (etat == Etat.Debout) // S'assoir puisqu'il est debout
                {
                    MoveAmount = Vector3.zero;
                    Anim.Set(HumanAnim.Type.Sit);
                    etat = Etat.Assis;
                }
                else // Se lever depuis assis
                {
                    Anim.Stop(HumanAnim.Type.Sit);
                    etat = Etat.Debout;
                }

                LastChangementEtat = Time.time;
            }
            else if (touches.GetKey(TypeTouche.Accroupi) && etat != Etat.Assis && Time.time - LastChangementEtat > 0.5f) // il ne doit pas être assis
            {
                if (etat == Etat.Debout) // s'accroupir puisqu'il est debout
                {
                    Anim.Set(HumanAnim.Type.Squat);
                    etat = Etat.Accroupi;
                }
                else // se lever depuis accroupi
                {
                    Anim.Stop(HumanAnim.Type.Squat);
                    etat = Etat.Debout;
                }
                
                LastChangementEtat = Time.time;
            }
            else if (etat == Etat.Assis || Time.time - LastChangementEtat < 0.25f) // Aucune animation lorsque le chassé est assis et s'assois/s'accroupi
            {}
            else if (etat == Etat.Accroupi)
            {
                if (zMov == -1)// reculer
                {
                    Anim.Set(HumanAnim.Type.Backward);
                }
                if (zMov == 1 || xMov != 0) // avancer
                {
                    Anim.Set(HumanAnim.Type.Forward);
                }
            }
            else if (zMov == 1 && xMov == 1) // diagonale droite
            {
                Anim.Set(HumanAnim.Type.DiagR);
            }
            else if (zMov == 1 && xMov == -1) // diagonale gauche
            {
                Anim.Set(HumanAnim.Type.DiagL);
            }
            else if (zMov == 1) // Avancer
            {
                if (xMov == 0 && touches.GetKey(TypeTouche.Sprint)) // Sprinter
                {
                    Anim.Set(HumanAnim.Type.Run);
                }
                else // Avancer normalement
                {
                    Anim.Set(HumanAnim.Type.Forward);
                }
            }
            else if (zMov == -1) // Reculer
            {
                Anim.Set(HumanAnim.Type.Backward);
            }
            else if (xMov == 1) // Droite
            {
                Anim.Set(HumanAnim.Type.Right);
            }
            else if (xMov == -1) // Gauche
            {
                Anim.Set(HumanAnim.Type.Left);
            }
            else
            {
                Anim.StopContinue();
            }
        }
    }
}
