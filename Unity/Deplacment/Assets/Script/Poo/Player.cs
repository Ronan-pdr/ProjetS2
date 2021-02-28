using System;
using UnityEngine;

namespace Script
{
    public abstract class Player : Humanoide
    {
        //Déplacement
        private string toucheAvancer = "z";
        private string toucheReculer = "s";
        private string toucheDroite = "d";
        private string toucheGauche = "q";
        private float speed = 4f;
        private float lookSensitivity = 5f;
        private string toucheJump = "space";
        private int forceJump = 15000;

        //Caméra
        protected Transform Cam;
        
        protected void MovementPlayer()
        {
            //Récupérer toutes les valeurs des déplacements
            Vector3 velocity, rotation, cameraRotation;
            (velocity, rotation, cameraRotation) = CalculMove();
            
            //Modifier 'transform' du joueur et de la caméra
            Rb.MovePosition(Rb.position + velocity * Time.fixedDeltaTime);
            Rb.MoveRotation(Rb.rotation * Quaternion.Euler(rotation));
            Cam.transform.Rotate(cameraRotation);
            
            //Jump
            if (Input.GetKey(toucheJump) && Ground)
            {
                Debug.Log("I jump !");
                Rb.AddForce(0, forceJump * Time.deltaTime, 0);
                Ground = false;
            }
        }

        private (Vector3, Vector3, Vector3) CalculMove()
        {
            //Droite, Gauche
            int xMov = 0;
            if (Input.GetKey(toucheDroite))
                xMov++;
            
            if (Input.GetKey(toucheGauche))
                xMov--;
            
            //Avancer, Reculer
            int zMov = 0;
            if (Input.GetKey(toucheAvancer))
                zMov++;
            
            if (Input.GetKey(toucheReculer))
                zMov--;
        
            //Arranger ça avec la rotation (en gros ça fait les sinus et cosinus à notre place)
            Vector3 moveHorizontal = Tr.right * xMov;
            Vector3 moveVertical = Tr.forward * zMov;
            
            Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;
            
            //Rotation sur y
            float yRot = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
            
            //Rotation caméra sur x
            float xRot = Input.GetAxisRaw("Mouse Y");
            Vector3 cameraRotation = new Vector3(-xRot, 0f, 0f) * lookSensitivity;

            return (velocity, rotation, cameraRotation);
        }
    }
}