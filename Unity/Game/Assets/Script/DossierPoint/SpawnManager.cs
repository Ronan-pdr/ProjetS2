using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.DossierPoint
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        
        private List<Point> spawnPointsChasseur;
        private List<Point> spawnPointsChassé;
        
        // Getter
        public int GetLengthSpawnPointChasseur() => spawnPointsChasseur.Count;
        public int GetLengthSpawnPointChassé() => spawnPointsChassé.Count;
        public Transform GetTrSpawnPointChasseur(int index) => spawnPointsChasseur[index].transform;
        public Transform GetTrSpawnPointChassé(int index) => spawnPointsChassé[index].transform;
    
        private void Awake()
        {
            Instance = this;

            spawnPointsChasseur = new List<Point>();
            spawnPointsChassé = new List<Point>();

            Point[] points = GetComponentsInChildren<Point>();
            int len = points.Length;

            int i;
            for (i = 0; i < len && points[i].name != "SpawnChassé"; i++)
            {
                spawnPointsChasseur.Add(points[i]);
            }

            for (; i < len; i++)
            {
                spawnPointsChassé.Add(points[i]);
            }
        }
    }
}

