using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Tools
{
    public class ManList
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
    }
}


