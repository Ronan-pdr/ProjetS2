using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossManager : MonoBehaviour
{
    public static CrossManager Instance;
    private Point[] crossPoints;

    private void Awake()
    {
        Instance = this;
        crossPoints = GetComponentsInChildren<Point>();
    }

    public (Vector3, int) GetPosition(int previousIndex)
    {
        int len = crossPoints.Length;
        int max = len;
        if (previousIndex != -1)
            max -= 1;

        int index = Random.Range(0, max);
        if (index == previousIndex)
            index = SimpleMath.Mod(index + 1, len);
        
        return (crossPoints[index].transform.position, index);
    }
}
