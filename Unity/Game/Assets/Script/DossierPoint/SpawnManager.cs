using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.DossierPoint
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        private Point[] spawnPoints;
    
        private void Awake()
        {
            Instance = this;
            spawnPoints = GetComponentsInChildren<Point>();
        }
    
        public Transform GetSpawnPoint()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        }
    }
}

