using System.Runtime.Serialization;

namespace Script.Tools
{
    public class ManString
    {
        // ex : Format("t", 3) -> "  t"
        public static string Format(string s, int taille)
        {
            for (int i = taille - s.Length; i > 0; i--)
            {
                s = " " + s;
            }

            return s;
        }
    }
}