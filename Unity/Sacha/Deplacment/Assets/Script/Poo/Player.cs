using UnityEngine;

namespace Script
{
    public abstract class Player : Humanoide
    {
        private string toucheAvancer = "z";
        private string toucheReculer = "s";
        private string toucheDroite = "d";
        private string toucheGauche = "q";
        private int forceMovement = 1000;
        
        private string toucheJump = "v";
        private int forceJump = 15000;
        
        
        protected void MovementPlayer()
        {
            if (Input.GetKey(toucheAvancer))
            {
                Rg.AddForce(-forceMovement * Time.deltaTime, 0, 0);
            }
            
            if (Input.GetKey(toucheReculer))
            {
                Rg.AddForce(forceMovement * Time.deltaTime, 0, 0);
            }
            
            if (Input.GetKey(toucheDroite))
            {
                Rg.AddForce(0, 0, forceMovement * Time.deltaTime);
            }
            
            if (Input.GetKey(toucheGauche))
            {
                Rg.AddForce(0, 0, -forceMovement * Time.deltaTime);
            }

            if (Input.GetKey(toucheJump) && Ground)
            {
                Debug.Log("I jump !");
                Rg.AddForce(0, forceJump * Time.deltaTime, 0);
                Ground = false;
            }
        }
    }
}