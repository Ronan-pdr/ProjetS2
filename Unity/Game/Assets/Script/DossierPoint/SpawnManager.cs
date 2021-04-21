using System;
using System.Collections.Generic;
using Photon.Pun;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using Random = System.Random;

namespace Script.DossierPoint
{
    public class SpawnManager : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        public static SpawnManager Instance;

        private SpawnPoint[] spawnChasseur;
        private SpawnPoint[] spawnChassé;
        
        // ------------ Getters ------------
        public int GetNbSpawnChasseur() => spawnChasseur.Length;
        public int GetNbSpawnChassé() => spawnChassé.Length;
        public Transform GetTrChasseur(int index) => spawnChasseur[index].transform;
        public Transform GetTrChassé(int index) => spawnChassé[index].transform;
        
        // ------------ Constructeur ------------
        private void Awake()
        {
            Instance = this;
            RecupSpawns();
        }

        // ------------ Méthodes ------------
        private void RecupSpawns()
        {
            // les listes où sont temporairements stockées tous les spawnPoints
            SpawnPoint[] points = GetComponentsInChildren<SpawnPoint>();
            List<SpawnPoint> spawnBeginChasseur = new List<SpawnPoint>();
            List<SpawnPoint> spawnBeginChassé = new List<SpawnPoint>();

            int i;
            int len = points.Length;
            for (i = 0; i < len; i++)
            {
                if (points[i].IsChasseurSpawn())
                {
                    spawnBeginChasseur.Add(points[i]);
                }
                else if (points[i].IsChasséSpawn())
                {
                    spawnBeginChassé.Add(points[i]);
                }
                else
                {
                    throw new Exception($"Le spawn {points[i].Typo} n'est pas encore répétorié");
                }
            }

            spawnChasseur = ManList<SpawnPoint>.Copy(spawnBeginChasseur);
            spawnChassé = ManList<SpawnPoint>.Copy(spawnBeginChassé);
        }

        public int[] GetBeginPosition(TypePlayer typePlayer)
        {
            switch (typePlayer)
            {
                case TypePlayer.Chasseur:
                    return Aux(spawnChasseur.Length);
                case TypePlayer.Chassé:
                    return Aux(spawnChassé.Length);
                default:
                    throw new Exception($"Pas de spawn pour {typePlayer}");
            }
            
            int[] Aux(int length)
            {
                return ManList.RandomIndex(length);
            }
        }
    }
}