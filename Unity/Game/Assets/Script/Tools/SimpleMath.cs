using System;
using UnityEngine;
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

        public static Vector3 Mod(Vector3 v, float f)
        {
            return new Vector3(v.x % f, v.y % f, v.z % f);
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
            float rad = DegreToRadian(angle);
            return (float)Math.Cos(rad);
        }
        
        public static float Sin(float angle)
        {
            float rad = DegreToRadian(angle);
            return (float)Math.Sin(rad);
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

        public static bool IsEncadré(Vector3 v, Vector3 e)
        {
            return e.x - 0.5f < v.x && v.x < e.x + 0.5f 
                && e.y - 0.5f < v.y && v.y < e.y + 0.5f
                && e.z - 0.5f < v.z && v.z < e.z + 0.5f;
        }
        
        public static bool IsEncadré(float a, float b)
        {
            return b - 0.5f < a && a < b + 0.5f;
        }
    }    
}