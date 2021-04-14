using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Tools
{
    public static class ManList
    {
        // ex : CreateListRange(4) -> [0, 1, 2, 3]
        public static List<int> CreateListRange(int length)
        {
            List<int> res = new List<int>();
            
            for (int i = 0; i < length; i++)
            {
                res.Add(i);
            }

            return res;
        }

        public enum Coord
        {
            X,
            Y,
            Z
        }

        private static float GetCoord(Vector3 v, Coord coord)
        {
            if (coord == Coord.X)
                return v.x;
            if (coord == Coord.Y)
                return v.y;
            return v.z;
        }

        public static float GetMax(Vector3[] positions, Coord coord)
        {
            int l = positions.Length;
            if (l == 0)
            {
                throw new Exception("Pas de max dans une array vide");
            }
            
            float max = GetCoord(positions[0], coord);
            for (int i = 1; i < l; i++)
            {
                if (GetCoord(positions[0], coord) > max)
                {
                    max = GetCoord(positions[0], coord);
                }
            }

            return max;
        }
        
        public static float GetMin(Vector3[] positions, Coord coord)
        {
            int l = positions.Length;
            if (l == 0)
            {
                throw new Exception("Pas de min dans une array vide");
            }
            
            float min = GetCoord(positions[0], coord);
            for (int i = 1; i < l; i++)
            {
                if (GetCoord(positions[0], coord) < min)
                {
                    min = GetCoord(positions[0], coord);
                }
            }

            return min;
        }
    }
}