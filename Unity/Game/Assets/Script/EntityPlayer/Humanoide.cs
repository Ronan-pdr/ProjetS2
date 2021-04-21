using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Bot;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Script.EntityPlayer
{
    public abstract class Humanoide : Entity
    {
        // ------------Etat ------------
        protected bool Grounded;

        // Avancer
        protected const float WalkSpeed = 3f;
        protected const float SprintSpeed = 5f;

        // Jump
        private const float JumpForce = 200f;
    
        // Vie
        protected int MaxHealth;
        protected int CurrentHealth;
    
        // photon
        protected PhotonView Pv;
        
        // Jump 
        private float lastJump; // le temps la dernière fois que le joueur a sauté
        private float periodeJump = 0.2f; // tous les combien de temps il peut sauter
    
        // ------------ Getters ------------
        public int GetCurrentHealth() => CurrentHealth;
        public int GetMaxHealth() => MaxHealth;
        public PhotonView GetPv() => Pv;
        public Player GetPlayer() => Pv.Owner;
        
        // ------------ Setters ------------
        public void SetGroundedState(bool value)
        {
            Grounded = value;
        }

        // ------------ Constructeurs ------------
        protected void AwakeHuman()
        {
            Pv = GetComponent<PhotonView>(); // doit obligatoirement être dans awake
        }

        protected void StartHuman()
        {
            CurrentHealth = MaxHealth;
        }
        
        // ------------ Update ------------
        
        protected void UpdateHumanoide()
        {
            PotentielleMort();
        }

        // ------------ Méthodes ------------
        private void PotentielleMort()
        {
            // Mourir de chute
            if (transform.position.y < -10f)
            {
                Die();
            }
        
            // Mourir point de vie
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Jump()
        {
            if (Time.time - lastJump > periodeJump && Grounded)
            {
                Rb.AddForce(transform.up * JumpForce); // transform.up = new Vector3(0, 1, 0)
                Grounded = false;
                lastJump = Time.time;
            }
        }
        
        // Aucune information pertinente ne peut être retenu
        // du script qui appelle cette fonction
        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
        
            Hashtable hash = new Hashtable();

            if (this is PlayerClass)
            {
                hash.Add("PointDeViePlayer", CurrentHealth);
            }
            else
            {
                // comme il faut indiqué la vie ainsi que le bot à qui c'est concerné, on met les deux infos dans une string
                string mes = ((BotClass) this).EncodeFormatVieBot();
                hash.Add("PointDeVieBot", mes);
            }

            Pv.Owner.SetCustomProperties(hash);
        }

        protected abstract void Die();

        // ------------ Animation ------------
        
        [Header("Animation")]
        [SerializeField] protected Animator anim;

        protected void AnimationStop()
        {
            anim.enabled = false;
        }
        
        protected void ActiverAnimation(string strAnimation)
        {
            anim.enabled = true;
            anim.Play(strAnimation);
        }
        
        // ------------ Surchargeurs ------------

        public static bool operator ==(Humanoide hum1, Humanoide hum2)
        {
            if (hum1 is null || hum2 is null)
            {
                //Debug.Log("WARNING : Tu as testé l'égalité de deux humains dont au moins un est null");
                //Debug.Log($"hum1 -> {hum1} ; hum2 -> {hum2}");
                return false;
            }
            
            try
            {
                return hum1.name == hum2.name;
            }
            catch (Exception e)
            {
                if (false) // grâce à ça j'ai plus de WARNING bordel
                    Debug.Log(e);
                
                return false;
            }
        }
        public static bool operator !=(Humanoide hum1, Humanoide hum2)
        {
            return !(hum1 == hum2);
        }

        public override bool Equals(object other)
        {
            if (other is Humanoide)
                return this == (Humanoide)other;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

