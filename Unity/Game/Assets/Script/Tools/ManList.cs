using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Script.Tools
{
    public static class ManList
    {
        // constructeurs
        
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

        // manipulation

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
        
        public static int[] RandomIndex(int length)
        {
            int[] arr = new int[length];
            List<int> list = CreateListRange(length);
            Random random = new Random();

            for (int iArr = 0; iArr < length; iArr++)
            {
                int iList = random.Next(list.Count);
                int n = list[iList];
                list.RemoveAt(iList);

                arr[iArr] = n;
            }

            return arr;
        }
    }
    
    public static class ManList<T>
    {
        // contructeur
        public static T[] Copy(List<T> list)
        {
            int l = list.Count;
            T[] arr = new T[l];

            for (int i = 0; i < l; i++)
            {
                arr[i] = list[i];
            }

            return arr;
        }
    }
}