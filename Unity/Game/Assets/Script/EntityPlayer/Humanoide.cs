using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Script.EntityPlayer
{
    public abstract class Humanoide : Entity
    {
        //Etat
        protected bool Grounded;

        //Avancer
        protected const float WalkSpeed = 3f;
        protected const float SprintSpeed = 5f;

        //Jump
        private const float JumpForce = 200f;
    
        //GamePlay
        protected int MaxHealth;
        protected int CurrentHealth;
    
        // photon
        protected PhotonView Pv;
    
        //Getter
        public int GetCurrentHealth() => CurrentHealth;
        public int GetMaxHealth() => MaxHealth;
        public PhotonView GetPv() => Pv;
        public Player GetPlayer() => Pv.Owner;
        
        public void SetGroundedState(bool grounded)
        {
            Grounded = grounded;
        }

        protected void AwakeHuman()
        {
            Pv = GetComponent<PhotonView>(); // doit obligatoirement être dans awake
        }

        protected void StartHuman()
        {
            CurrentHealth = MaxHealth;
        }

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

        protected void UpdateHumanoide()
        {
            PotentielleMort();
        }
    
        //Déplacment

        protected void JumpHumanoide()
        {
            Rb.AddForce(transform.up * JumpForce); // transform.up = new Vector3(0, 1, 0)
            Grounded = false;
        }

        //Animation
        [SerializeField] protected Animator anim;

        protected void AnimationStop()
        {
            anim.enabled = false;
        }
    
        //GamePlay
        public void TakeDamage(int damage) // Seul les chasseurs activent cette fonction
        {
            CurrentHealth -= damage;
        
            Hashtable hash = new Hashtable();

            if (this is PlayerClass)
            {
                hash.Add("PointDeViePlayer", CurrentHealth);
            }
            else
            {
                string mes = CurrentHealth.ToString(); // comme il faut indiqué la vie ainsi que le bot à qui c'est concerné, on met les deux infos dans une string
                while (mes.Length < 3) // on formate la vie à trois charactères
                {
                    mes = " " + mes;
                }
            
                hash.Add("PointDeVieBot", name + mes);
            }

            Pv.Owner.SetCustomProperties(hash);
        }

        protected abstract void Die();

        public static bool operator ==(Humanoide hum1, Humanoide hum2)
        {
            if (hum1 is null || hum2 is null)
                throw new Exception("Tu as testé l'égalité de deux humains dont au moins un est null");
            
            return hum1.name == hum2.name;
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

