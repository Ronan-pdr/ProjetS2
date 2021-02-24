using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABasics : IAClass
{
    private int previousIndex;
    private Vector3 destination;

    void Awake()
    {
        AwakeIA();
    }

    void Start()
    {
        (Vector3 coord, int index) = CrossManager.Instance.GetPosition(-1);
        Tr.position += coord;
        previousIndex = index;
    }

    void Update()
    {
        
    }
}
