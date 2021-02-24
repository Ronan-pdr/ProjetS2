using UnityEngine;

namespace Script.Tools
{
    public class Calcul
    {
        public static float Distance(Vector3 a, Vector3 b)
        {
            float x = SimpleMath.Pow(SimpleMath.Abs(a.x - b.x), 2);
            float y = SimpleMath.Pow(SimpleMath.Abs(a.y - b.y), 2);
            float z = SimpleMath.Pow(SimpleMath.Abs(a.z - b.z), 2);

            return SimpleMath.Sqrt(x + y + z);
        }
    }
}