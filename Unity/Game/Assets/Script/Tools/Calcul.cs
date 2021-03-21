using UnityEngine;

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

        public static Vector3 Diff(Vector3 destination, Vector3 depart)
        {
            float diffX = destination.x - depart.x;
            float diffY = destination.y - depart.y;
            float diffZ = destination.z - depart.z;
            
            return new Vector3(diffX, diffY, diffZ);
        }

        // Calcul l'angle le plus faible pour qu'un objet à la position 'depart'
        // puisse s'orienter face à 'destination'. A noter que ce sera un angle cohérent seulement sur 'coord'.
        // 'rotationInitiale' DOIT être en degré et correspond à la rotation sur 'coord' de l'objet aux
        // coordonnées 'départ'
        public static float Angle(float rotationInitiale, Vector3 depart, Vector3 destination, Coord coord)
        {
            float diffX, diffY, diffZ;
            Vector3 vect = Diff(destination, depart);
            (diffX, diffY, diffZ) = (vect.x, vect.y, vect.z);

            float adjacent, opposé;
            if (coord == Coord.X)
            {
                opposé = diffZ;
                adjacent = diffY;
            }
            else if (coord == Coord.Y)
            {
                opposé = diffX;
                adjacent = diffZ;
            }
            else // pas défini
            {
                opposé = 0;
                adjacent = 0;
            }
                

            float amountRotation;
            if (opposé == 0)
            {
                if (adjacent > 0)
                    amountRotation = 0;
                else
                    amountRotation = 180;
            }
            else if (adjacent == 0) // on ne peut pas diviser par 0 donc je suis obligé de faire ce cas (dans le ArcTan)
            {
                if (opposé < 0)
                    amountRotation = -90;
                else
                    amountRotation = 90;
            }
            else
            {
                amountRotation = SimpleMath.ArcTan(opposé, adjacent); // amountRotation : Df = ]-90, 90[
            
                if (adjacent < 0) // Fait quatre schémas avec les différentes configurations pour comprendre
                {
                    if (opposé < 0)
                        amountRotation -= 180; // amountRotation était positif
                    else
                        amountRotation += 180; // amountRotation était négatif
                }
            }
        
            // On doit ajouter sa rotation initiale à la rotation qu'il devait faire s'il était à 0 degré
            amountRotation -= rotationInitiale; // eulerAngles pour récupérer l'angle en degré

            if (amountRotation > 180) // Le degré est déjè valide, seulement, il est préférable de tourner de -150° que de 210° (par exemple)
                amountRotation -= 360;
            else if (amountRotation < -180)
                amountRotation += 360;

            return amountRotation;
        }
    }
}