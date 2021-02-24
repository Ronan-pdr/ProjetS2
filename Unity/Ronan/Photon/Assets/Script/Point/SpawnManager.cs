using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private SpawntPoint[] spawnPoints;

    private void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawntPoint>();
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
    }
}
