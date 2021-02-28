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

    public (Vector3, int) GetRandomPosition(int previousIndex)
    {
        int len = crossPoints.Length;
        int max = len;
        if (previousIndex == -1) //Si bot vient d'aparaître
            previousIndex = len;
        else
            max -= 1;
            

        int index = Random.Range(0, max);
        if (index >= previousIndex)
            index++;
        
        return (crossPoints[index].transform.position, index);
    }

    public Vector3 GetPosition(int index)
    {
        if (index >= crossPoints.Length)
        {
            Debug.Log("GetPostion cross manager : index out of range");
            return Vector3.zero;
        }

        return crossPoints[index].transform.position;
    }
}
