﻿using UnityEngine;

namespace Script.Tools
{
    public class Calcul
    {
        public enum Coord
        {
            X,
            Y,
            Z
        }
        
        // distance entre deux points à deux coordonnées
        public static float Distance(Vector3 a, Vector3 b)
        {
            float x = SimpleMath.Abs(a.x - b.x);
            float y = SimpleMath.Abs(a.y - b.y);
            float z = SimpleMath.Abs(a.z - b.z);

            return Norme(x, y, z);
        }

        public static float Distance(Vector3 a, Vector3 b, Coord coordWithout) // 
        {
            (float x, float y, float z) = (0, 0, 0);
            if (coordWithout != Coord.X)
                x = SimpleMath.Abs(a.x - b.x);
            if (coordWithout != Coord.Y)
                y = SimpleMath.Abs(a.y - b.y);
            if (coordWithout != Coord.Z)
                z = SimpleMath.Abs(a.z - b.z);
            
            return Norme(x, y, z);
        }

        // distance entre deux points à une coordonnée
        public static float Distance(float a, float b)
        {
            return SimpleMath.Abs(a - b);
        }

        public static float Norme(float x, float y, float z)
        {
            float a = SimpleMath.Pow(x, 2);
            float b = SimpleMath.Pow(y, 2);
            float c = SimpleMath.Pow(z, 2);

            return SimpleMath.Sqrt(a + b + c);
        }

        // Calcul l'angle le plus faible pour qu'un objet au 'depart' (avec un angle de 'rotationInitiale' DEGRE)
        // puisse s'orienter face à 'destination'. A noter que ce sera un angle cohérent seulement sur y
        // 'rotationInitiale' DOIT être en degré et correspond à la rotation sur y de l'objet aux
        // coordonnées départs
        public static float Angle(float rotationInitialeY, Vector3 depart, Vector3 destination)
        {
            float diffX = destination.x - depart.x;
            float diffZ = destination.z - depart.z;

            float amountRotation;
            if (diffX == 0) // on ne peut pas diviser par 0 donc je suis obligé de faire ce cas (dans le ArcTan)
            {
                amountRotation = 0;
            }
            else if (diffZ == 0)
            {
                if (diffX < 0)
                    amountRotation = -90;
                else
                    amountRotation = 90;
            }
            else
            {
                amountRotation = SimpleMath.ArcTan(diffX, diffZ); // amountRotation : Df = ]-90, 90[
            
                if (diffZ < 0) // Fait quatre schémas avec les différentes configurations pour comprendre
                {
                    if (diffX < 0)
                        amountRotation -= 180; // amountRotation était positif
                    else
                        amountRotation += 180; // amountRotation était négatif
                }
            }
        
            // On doit ajouter sa rotation initiale à la rotation qu'il devait faire s'il était à 0 degré
            amountRotation -= rotationInitialeY; // eulerAngles pour récupérer l'angle en degré

            if (amountRotation > 180) // Le degré est déjè valide, seulement, il est préférable de tourner de -150° que de 210° (par exemple)
                amountRotation -= 360;
            else if (amountRotation < -180)
                amountRotation += 360;

            return amountRotation;
        }
    }
}