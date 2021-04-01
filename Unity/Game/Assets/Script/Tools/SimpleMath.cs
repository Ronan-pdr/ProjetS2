using System;
using static System.Math;

namespace Script.Tools
{
    public class SimpleMath
    {
        public static int Mod(int a, int b)
        {
            int r = a % b;
            if (r < 0)
                return b + r;
            return r;
        }
    
        public static float Abs(float a)
        {
            if (a < 0)
                return -a;
            return a;
        }
    
        public static float Pow(float x, int n)
        {
            float res = 1;
            for (int i = 0; i < n; i++)
            {
                res *= x;
            }
    
            return res;
        }
    
        public static float Sqrt(float a)
        {
            return (float)Math.Sqrt(a);
        }
    
        public static float Cos(float angle)
        {
            float degre = DegreToRadian(angle);
            return (float)Math.Cos(degre);
        }
        
        public static float Sin(float angle)
        {
            float degre = DegreToRadian(angle);
            return (float)Math.Sin(degre);
        }
    
        public static float ArcTan(float opposé, float adjacent) // l'angle obtenu sera toujours positif
        {
            if (adjacent == 0)
                throw new DivideByZeroException("On ne peut pas diviser par 0 (ArcTan)");
                
            float r = (float) Atan(opposé / adjacent); //r est l'angle en radian
            return RadianToDegre(r);
        }
    
        public static float RadianToDegre(float angle)
        {
            return (float) (angle * 360 / (2 * PI));
        }

        public static float DegreToRadian(float angle)
        {
            return (float) (angle * 2 * PI / 360);
        }
    }    
}