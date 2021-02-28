using UnityEngine;

namespace Script.Tools
{
    public class Calcul
    {
        public static float Distance(Vector3 a, Vector3 b)
        {
            float x = SimpleMath.Abs(a.x - b.x);
            float y = SimpleMath.Abs(a.y - b.y);
            float z = SimpleMath.Abs(a.z - b.z);

            return Norme(x, y, z);
        }

        public static float Norme(float x, float y, float z)
        {
            float a = SimpleMath.Pow(x, 2);
            float b = SimpleMath.Pow(y, 2);
            float c = SimpleMath.Pow(z, 2);

            return SimpleMath.Sqrt(a + b + c);
        }
    }
}