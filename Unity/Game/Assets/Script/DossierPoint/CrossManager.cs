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
        // ------------ Attributs ------------
        
        public static CrossManager Instance;
        private CrossPoint[] allCrossPoints;
        
        private string Path = "Build/SauvegardeCrossManager/";

        // ------------ Getters ------------
        public string GetPath() => Path;
        public int GetNumberPoint() => allCrossPoints.Length;
        
        public CrossPoint GetPoint(int index) => allCrossPoints[index];

        public CrossPoint[] GetCrossPoints() => allCrossPoints;
        
        // pour l'instant c'est pas random
        public int[] GetSpawnBot() => ManList.CreateArrRange(allCrossPoints.Length);

        public Vector3 GetPosition(int index)
        {
            if (index >= allCrossPoints.Length)
            {
                throw new Exception("index out of range");
            }
            
            return allCrossPoints[index].transform.position;
        }

        // ------------ Setters ------------
        
        private void SetCrossPoint()
        {
            // Range tous les crossPoints en fonction de leur nom (il y a un numéro dedans)
            // Vérifie si aucun crossPoint n'a le même numéro qu'un autre
            
            CrossPoint[] crossPoints = GetComponentsInChildren<CrossPoint>();
            int l = crossPoints.Length;

            allCrossPoints = new CrossPoint[l];

            foreach (CrossPoint crossPoint in crossPoints)
            {
                int index = CrossPoint.NameToIndex(crossPoint.name);

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

            if (!CrossMaintenance.Instance.IsMaintenance)
            {
                LoadNeigboors();
            }
        }
        
        // ------------ Méthodes ------------

        

        // ------------ Parsing ------------

        public void LoadNeigboors()
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
                    Debug.Log($"WARNING : Le fichier de sauvegarde '{sousCrossManager.name}' des crossPoints n'existe pas --> faire une maintenance");
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
                        int iCrossPoint = CrossPoint.NameToIndex(nameCrossPoint);

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