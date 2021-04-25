using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Photon.Pun;
using Script.EntityPlayer;
using Script.InterfaceInGame;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Script.DossierPoint
{
    public class CrossManager : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Maintenance")]
        [SerializeField] private bool InMaintenance;
        [SerializeField] private SousCrossManager _sousCrossManager;
        
        // ------------ Attributs ------------
        
        public static CrossManager Instance;
        private CrossPoint[] allCrossPoints;
        
        // 'indexResearch'est utilisé lors de la maintenance pour savoir quel est le 
        // cross point qui doit chercher ses voisins
        private int indexResearch;

        private string[] contentOutput;
        
        private string Path = "Build/SauvegardeCrossManager/";
        private string nameFile;
        private CrossPoint[] _maintenanceCrossPoints;
        private int nNewNeighboor;
        
        // calculer la complexité
        private Stopwatch time;
        
        // ------------ Getters ------------
        public int GetNumberPoint() => allCrossPoints.Length;
        
        public CrossPoint GetPoint(int index) => allCrossPoints[index];

        public CrossPoint[] GetCrossPoints() => allCrossPoints;
        
        // pour l'instant c'est pas random
        public int[] GetSpawnBot() => ManList.CreateArrRange(allCrossPoints.Length);

        public bool IsMaintenance() => InMaintenance;
    
        public Vector3 GetPosition(int index)
        {
            if (index >= allCrossPoints.Length)
            {
                throw new Exception("index out of range");
            }
            
            return allCrossPoints[index].transform.position;
        }

        // ------------ Setters ------------

        public void OneNewNeighboorFind()
        {
            nNewNeighboor += 1;
        }
        
        private void SetCrossPoint()
        {
            CrossPoint[] crossPoints = GetComponentsInChildren<CrossPoint>();
            int l = crossPoints.Length;

            allCrossPoints = new CrossPoint[l];

            foreach (CrossPoint crossPoint in crossPoints)
            {
                int index = CrossPoint.GetIndexToName(crossPoint.name);

                if (!(allCrossPoints[index] is null))
                {
                    throw new Exception($"Deux cross point ont le même numéro ({index})");
                }
                    
                allCrossPoints[index] = crossPoint;
            }
        }
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;
            SetCrossPoint();
            
            Debug.Log($"lenght = {allCrossPoints.Length}");
            foreach (CrossPoint crossPoint in allCrossPoints)
            {
                if (crossPoint is null)
                {
                    throw new Exception("La liste allCrossPoints a mal été créé");
                }
            }
        }
        private void Start()
        {
            if (MasterManager.Instance.GetTypeScene() == MasterManager.TypeScene.Labyrinthe)
                return;

            if (InMaintenance)
            {
                BeginMaintenance();
            }
            else
            {
                LoadNeigboors(nameScMan => throw new Exception($"Le fichier de sauvegarde '{nameScMan}' des crossPoints n'existe pas --> faire une maintenance"));
            }
        }
        
        // ------------ Méthodes ------------

        private void BeginMaintenance()
        {
            LoadNeigboors(nameScMan => Debug.Log($"{nameScMan} n'a encore jamais été initialisé"));
                
            _maintenanceCrossPoints = _sousCrossManager.CrossPoints;
            nameFile = _sousCrossManager.name;
                
            contentOutput = new string[_maintenanceCrossPoints.Length];
                
            // Let's the party started
            indexResearch = -1;
            NextResearch();
                
            // enregistrer temps au début de la recherche
            time = new Stopwatch();
            time.Start();
        }

        public void EndOfOneResearch(List<CrossPoint> neighboors)
        {
            if (indexResearch >= _maintenanceCrossPoints.Length)
                throw new Exception("Il y a plus de recherche que de CrossPoint...");
            
            contentOutput[indexResearch] = NeighboorsToContent(neighboors);
            
            NextResearch();
        }

        private void NextResearch()
        {
            indexResearch += 1;
            
            if (indexResearch < _maintenanceCrossPoints.Length)
            {
                _maintenanceCrossPoints[indexResearch].SearchNeighboors();
            }
            else // toutes les recherches ont été faites
            {
                Ouput();
                time.Stop();
                Debug.Log($"Maintenance de '{nameFile}' est terminé et a trouvé {nNewNeighboor} nouveaux voisins");
                Debug.Log($"La maintence s'est effectué en {time.ElapsedMilliseconds/60000} minutes et {time.ElapsedMilliseconds%60000/1000} secondes");

                // c'est reparti pour un tour
                if (nNewNeighboor > 0)
                {
                    BeginMaintenance();
                }
            }
        }
        
        private string NeighboorsToContent(List<CrossPoint> neighboors)
        {
            int nCrossPoint = allCrossPoints.Length;
            string res = $"{_maintenanceCrossPoints[indexResearch].name} : ";
            
            foreach (CrossPoint neighboor in neighboors)
            {
                int i;
                for (i = 0; i < nCrossPoint && neighboor != allCrossPoints[i]; i++)
                {}

                if (i == nCrossPoint)
                    throw new Exception($"{neighboor} n'est pas dans le crossManager");

                res += $",{i}";
            }

            return res;
        }

        // ------------ Parsing ------------
        private void Ouput()
        {
            // Créer le dossier s'il n'existe pas
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            using (StreamWriter sw = File.CreateText(Path + nameFile))
            {
                foreach (string ligne in contentOutput)
                {
                    sw.WriteLine(ligne);
                }
            }
        }

        private void LoadNeigboors(Action<string> notExist)
        {
            int l = allCrossPoints.Length;
            int nTraité = 0;

            foreach (SousCrossManager sousCrossManager in GetComponentsInChildren<SousCrossManager>())
            {
                if (File.Exists(Path + sousCrossManager.name))
                {
                    Aux(sousCrossManager.name);
                }
                else
                {
                    notExist(sousCrossManager.name);
                }
            }

            if (nTraité != l)
            {
                Debug.Log("WARNING : Le fichier de sauvegarde des crossPoints n'est pas compatible.");
                Debug.Log($"Il y a eu {nTraité} crossPoint traités pour {l} en tout --> faire une maintenance");
            }
            
            void Aux(string fileName)
            {
                using (StreamReader sr = File.OpenText(Path + fileName))
                {
                    string ligne;
                
                    while ((ligne = sr.ReadLine()) != null)
                    {
                        string[] infos = ligne.Split(',');

                        string nameCrossPoint = infos[0].Substring(0, infos[0].Length - 3);
                        int iCrossPoint = CrossPoint.GetIndexToName(nameCrossPoint);

                        if (iCrossPoint >= l)
                        {
                            throw new Exception(
                                $"WARNING : Le fichier de sauvegarde '{fileName}' des crossPoints n'est pas compatible --> faire une maintenance");
                        }

                        if (allCrossPoints[iCrossPoint].name != nameCrossPoint)
                        {
                            Debug.Log($"WARNING : Le fichier de sauvegarde '{fileName}' des crossPoints n'est pas compatible --> faire une maintenance");
                        }

                        // set les neighboors du cross point
                        int nInfo = infos.Length;
                        for (int i = 1; i < nInfo; i++)
                        {
                            allCrossPoints[iCrossPoint].AddNeighboor(allCrossPoints[int.Parse(infos[i])]);
                        }

                        nTraité++;
                    }
                }
            }
        }
    }
}