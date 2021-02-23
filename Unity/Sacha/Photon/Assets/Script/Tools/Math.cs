using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

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
        return (float)Math.Cos(angle);
    }
    
    public static float Sin(float angle)
    {
        return (float)Math.Sin(angle);
    }
}
