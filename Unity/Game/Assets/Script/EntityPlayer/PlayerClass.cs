using Photon.Pun;
using Photon.Realtime;
using Script.Bot;
using Script.Manager;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Script.EntityPlayer
{
    public abstract class PlayerClass : Humanoide
    {
        // Etat
        protected enum Etat
        {
            Debout,
            Assis,
            Accroupi
        }

        protected Etat etat = Etat.Debout;
        protected string touchLeverAssoir = "c";
        protected float LastChangementEtat; // La dernière fois qu'on a changé de position entre assis et lever
        protected string touchAccroupi = "x";

        //Avancer
        protected string touchAvancer = "z";
        protected string touchReculer = "s";
        protected string touchDroite = "d";
        protected string touchGauche = "q";
    
        //Sprint
        protected string touchSprint = "left shift";
    
        //Jump
        protected string touchJump = "space";

        //Look
        private float verticalLookRotation; 
        private float mouseSensitivity = 3f;
        [SerializeField] protected Transform cameraHolder;

        //Rassembler les infos
        protected Transform masterManager;
        
        protected void AwakePlayer()
        {
            SetRbTr();
            AwakeHuman();
        
            // parenter
            masterManager = MasterManager.Instance.transform;
            Tr.parent = masterManager;

            if (Pv.IsMine)
            {
                // indiquer quel est ton propre joueur au MasterManager
                MasterManager.Instance.SetOwnPlayer(this);
            }
        
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutPlayer(this);
        }

        protected void StartPlayer()
        {
            StartHuman();
            name = PhotonNetwork.NickName;
        
            if (!Pv.IsMine) 
            {
                Destroy(GetComponentInChildren<Camera>().gameObject); // On veut détruire les caméras qui ne sont pas les tiennes
                Destroy(Rb);
            }
        }

        protected void UpdatePlayer()
        {
            Look();
            Move();
            
            if (Input.GetKey(touchJump) && etat == Etat.Debout)
            {
                Jump();
            }
            
        
            UpdateHumanoide();
        }
    
        protected void FixedUpdatePlayer()
        {
            if (!Pv.IsMine)
                return;
        
            MoveEntity();
        }
    
        private void Move()
        {
            if (etat == Etat.Assis) // il se déplace pas quand il est assis
                return;
        
            int zMov = 0;
            if (Input.GetKey(touchAvancer))
                zMov++;
            if (Input.GetKey(touchReculer))
                zMov--;
            int xMov = 0;
            if (Input.GetKey(touchDroite))
                xMov++;
            if (Input.GetKey(touchGauche))
                xMov--;

            float speed = WalkSpeed;
            if (zMov == 1 && xMov == 0) // il faut qu'il avance tout droit pour sprinter
                speed = SprintSpeed;
            else if (xMov != 0 || zMov != 0) // en gros, s'il se déplace
            {
                etat = Etat.Debout; // il ne reste pas accroupi lorqu'il se déplace pas tout droit
            }

            Vector3 moveDir = new Vector3(xMov, 0, zMov);

            SetMoveAmount(moveDir, speed);
        }

        void Look()
        {
            Tr.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

            verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -70f, 30f);

            cameraHolder.localEulerAngles = Vector3.left * verticalLookRotation;
        }
    
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
        
            // les morts -> Die (MasterManager)
            if (!Pv.IsMine) // c'est le mourant qui envoie le hash
            {
                if (changedProps.TryGetValue("MortPlayer", out object value))
                {
                    Player player = (Player)value;
                    MasterManager.Instance.Die(player);
                }
            }
        }
        
        // Animation
        protected void Animation()
        {
            (int xMov, int zMov) = (0, 0);
            if (Input.GetKey(touchAvancer)) // avancer
                zMov += 1;
            if (Input.GetKey(touchReculer)) // reculer
                zMov -= 1;
            if (Input.GetKey(touchDroite)) // droite
                xMov += 1;
            if (Input.GetKey(touchGauche)) // gauche
                xMov -= 1;


            if (Input.GetKey(touchLeverAssoir) && etat != Etat.Accroupi && Time.time - LastChangementEtat > 0.5f) // il ne doit pas être accroupi
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
            else if (Input.GetKey(touchAccroupi) && etat != Etat.Assis && Time.time - LastChangementEtat > 0.5f) // il ne doit pas être assis
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
            else if (etat == Etat.Assis || Time.time - LastChangementEtat < 0.25f) // Aucune animation lorsque le chassé est assis
            {}
            else if (zMov == 1) // Avancer
            {
                if (etat == Etat.Accroupi) // avancer en étant accroupi
                {
                    ActiverAnimation("Marche acc");
                }
                else if (xMov == 0 && Input.GetKey(touchSprint)) // Sprinter
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
            else if (Input.GetKey(touchJump)) // Jump
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
