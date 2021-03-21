using UnityEngine;
using Photon.Pun;
using Script.Bot;
using Script.InterfaceInGame;

namespace Script.EntityPlayer
{
    public class Chassé : PlayerClass
    {
        private void Awake()
        {
            AwakePlayer();
            
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutChassé(this);
        }

        void Start()
        {
            MaxHealth = 100;
            StartPlayer();
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

            UpdatePlayer();
            Animation();
        }

        private void FixedUpdate()
        {
            FixedUpdatePlayer();
        }
        
        //GamePlay

        protected override void Die() // Est appelé lorsqu'il vient de mourir
        {
            MasterManager.Instance.Die(Pv.Owner);
            
            anim.enabled = false;
            PhotonNetwork.Destroy(gameObject);
        }

        // Animation
        private void Animation()
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
        

        private void ActiverAnimation(string strAnimation)
        {
            anim.enabled = true;
            anim.Play(strAnimation);
        }
    }
}