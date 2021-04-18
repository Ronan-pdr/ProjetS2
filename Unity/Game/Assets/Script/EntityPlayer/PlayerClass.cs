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
        
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutPlayer(this);
            
            Fuyard.SetInfoCamera(this);
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
    }
}
