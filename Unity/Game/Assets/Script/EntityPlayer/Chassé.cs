using UnityEngine;
using Photon.Pun;
using System.IO;

namespace Script
{
    public class Chassé : PlayerClass
    {
        // le "gameObject" qui contient les graphiques (c'est plus un dossier qu'autre chose)
        [SerializeField] private GameObject graphics;
        
        private void Awake()
        {
            AwakePlayer();
        }

        void Start()
        {
            maxHealth = 100;
            StartPlayer();
        }
        
        void Update()
        {
            if (!PV.IsMine)
                return;
            
            Cursor.lockState = PauseMenu.Instance.GetIsPaused() ? CursorLockMode.None : CursorLockMode.Confined;
            Cursor.visible = PauseMenu.Instance.GetIsPaused();
            
            if (PauseMenu.Instance.GetIsPaused())
            {
                moveAmount = Vector3.zero;
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
            MasterManager.Instance.Die(_player);
            
            anim.enabled = false;
            PhotonNetwork.Destroy(gameObject);
        }

        // Animation
        private (string, string)[] arrAnim; // (touche, animation)

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


            if (Input.GetKey(touchLeverAssoir) && etatDebAssAcc != 2 && Time.time - lastChangementEtat > 0.5f) // il ne doit pas être accroupi
            {
                if (etatDebAssAcc == 0) // S'assoir puisqu'il est debout
                {
                    moveAmount = Vector3.zero;
                    ActiverAnimation("Assis");
                }
                else // Se lever depuis assis
                {
                    ActiverAnimation("Lever PASS");
                }

                lastChangementEtat = Time.time;
                etatDebAssAcc = 1 - etatDebAssAcc; // 0 -> 1 ou 1 -> 0
            }
            else if (Input.GetKey(touchAccroupi) && etatDebAssAcc != 1 && Time.time - lastChangementEtat > 0.5f) // il ne doit pas être assis
            {
                if (etatDebAssAcc == 0) // s'accroupir puisqu'il est debout
                {
                    moveAmount = Vector3.zero;
                    ActiverAnimation("Accroupir");
                }
                else // se lever depuis accroupi
                {
                    ActiverAnimation("Lever PAcc");
                }
                
                lastChangementEtat = Time.time;
                etatDebAssAcc = 2 - etatDebAssAcc; // 0 -> 2 ou 2 -> 0
            }
            else if (etatDebAssAcc == 1 || Time.time - lastChangementEtat < 0.25f) // Aucune animation lorsque le chassé est assis
            {}
            else if (zMov == 1) // Avancer
            {
                if (etatDebAssAcc == 2) // avancer en étant accroupi
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
        

        private void ActiverAnimation(string animation)
        {
            anim.enabled = true;
            anim.Play(animation);
        }
    }
}