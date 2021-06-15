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
        [SerializeField] private bool inMaintenance;
        [SerializeField] private SousCrossManager[] sousCrossManagers;
        
        // ------------ Attributs ------------
        
        public static CrossManager Instance;
        private CrossPoint[] allCrossPoints;
        
        private string DossierRangement = "SauvegardeCrossManager/";

        // ------------ Getters ------------
        public bool IsMaintenance => inMaintenance;
        public string GetDossier() => DossierRangement;
        public int GetNumberPoint() => allCrossPoints.Length;
        
        public CrossPoint GetPoint(int index) => allCrossPoints[index];

        public CrossPoint[] GetCrossPoints() => allCrossPoints;
        
        public int[] GetSpawnBot() => ManList.RandomIndex(allCrossPoints.Length);

        public Vector3 GetPosition(int index)
        {
            if (index >= allCrossPoints.Length)
            {
                throw new Exception("index out of range");
            }
            
            return allCrossPoints[index].transform.position;
        }

        // ------------ Setter ------------
        
        private void SetCrossPoint()
        {
            // Range tous les crossPoints en fonction de leur nom (il y a un numéro dedans)
            // Vérifie si aucun crossPoint n'a le même numéro qu'un autre
            
            CrossPoint[] crossPoints = GetComponentsInChildren<CrossPoint>();
            int l = crossPoints.Length;

            allCrossPoints = new CrossPoint[l];
            int printForError = 0;

            foreach (CrossPoint crossPoint in crossPoints)
            {
                int i = CrossPoint.NameToIndex(crossPoint.name);

                if (printForError > 0)
                {
                    Debug.Log($"{printForError}ème prochain = {crossPoint.name}");
                    printForError--;
                }

                if (i == -1)
                {
                    printForError = 2;
                }

                if (!(allCrossPoints[i] is null))
                {
                    Debug.Log($"Deux cross points ont le même numéro ({i})");
                    printForError = 2;
                }
                    
                allCrossPoints[i] = crossPoint;
            }
        }
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;
            
            if (!IsMaintenance)
            {
                // On NE doit PAS utiliser cette liste si c'est pas une maintenance
                sousCrossManagers = null;
            }
            
            SetCrossPoint();
            for (int i = 0; i < allCrossPoints.Length; i++)
            {
                if (allCrossPoints[i] is null)
                {
                    Debug.Log($"La liste allCrossPoints a mal été créé ou le cross point avec l'index '{i}' n'existe pas");
                }
            }
        }
        private void Start()
        {
            if (MasterManager.Instance.GetTypeScene() == MasterManager.TypeScene.Labyrinthe)
                return;

            if (IsMaintenance)
            {
                foreach (SousCrossManager e in sousCrossManagers)
                {
                    gameObject.AddComponent<CrossMaintenance>().SetSousCrossManager(e);
                }
            }
            else
            {
                foreach (SousCrossManager e in GetComponentsInChildren<SousCrossManager>())
                {
                    LoadNeigboors(e);
                }
            }
        }
        
        // ------------ Méthodes ------------

        

        // ------------ Parsing ------------

        public void LoadNeigboors(SousCrossManager sousCrossManager)
        {
            int l = allCrossPoints.Length;
            string fileName = sousCrossManager.name;

            string path = "";
            if (Directory.Exists("Build"))
            {
                path = "Build/";
            }
            
            path += DossierRangement + fileName;
            
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string ligne;
                
                    while ((ligne = sr.ReadLine()) != null)
                    {
                        string[] infos = ligne.Split(',');

                        string nameCrossPoint = infos[0].Substring(0, infos[0].Length - 3);
                        int iCrossPoint = CrossPoint.NameToIndex(nameCrossPoint);

                        if (iCrossPoint >= l)
                        {
                            throw new Exception($"WARNING : Le fichier de sauvegarde '{fileName}' des crossPoints n'est pas compatible --> faire une maintenance");
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
                    }
                }
            }
            else if (!IsMaintenance)
            {
                Debug.Log($"WARNING : Le fichier de sauvegarde '{sousCrossManager.name}' des crossPoints n'existe pas --> faire une maintenance");
            }
        }
    }
}