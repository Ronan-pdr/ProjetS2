using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Script.EntityPlayer;
using Script.InterfaceInGame;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.DossierPoint
{
    public class CrossManager : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        public static CrossManager Instance;
        private CrossPoint[] crossPoints;
        
        // 'indexResearch'est utilisé lors de la maintenance pour savoir quel est le 
        // cross point qui doit chercher ses voisins
        private int indexResearch;

        private string[] contentOutput;
        
        private string Path = "Build/SauvegardeCrossManager";
        
        // pour le spawn des botRectiline
        private List<int> indexPointLibre;
        
        // ------------ Getters ------------
        public int GetNumberPoint() => crossPoints.Length;
        
        public CrossPoint GetPoint(int index) => crossPoints[index];
        
        // pour l'instant c'est pas random
        public int[] GetSpawnBot() => ManList.CreateArrRange(crossPoints.Length);

        public Vector3 GetPositionSpawn()
        {
            if (indexPointLibre.Count == 0)
                throw new Exception("Pas asser de cross point pour spawn");

            Vector3 res = crossPoints[indexPointLibre[0]].transform.position;
            indexPointLibre.RemoveAt(0);

            return res;
        }
        
        public static bool IsMaintenance()
        {
            return PhotonNetwork.PlayerList.Length == 1 && 
                   PhotonNetwork.LocalPlayer.NickName == "maintenance" && 
                   MasterManager.Instance.GetTypeScene() == MasterManager.TypeScene.Game;
        }
    
        public Vector3 GetPosition(int index)
        {
            if (index >= crossPoints.Length)
            {
                throw new Exception("index out of range");
            }
            
            return crossPoints[index].transform.position;
        }
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;
            crossPoints = GetComponentsInChildren<CrossPoint>();

            int l = crossPoints.Length;
            indexPointLibre = ManList.CreateListRange(l);
            contentOutput = new string[l];
        }
        private void Start()
        {
            if (MasterManager.Instance.GetTypeScene() == MasterManager.TypeScene.Labyrinthe)
                return;

            if (MasterManager.Instance.IsInMaintenance())
            {
                indexResearch = -1;
                NextResearch();
            }
            else
            {
                LoadNeighboors();
            }
        }
        
        // ------------ Méthodes ------------

        public void EndOfOneResearch(List<CrossPoint> neighboors)
        {
            if (indexResearch >= crossPoints.Length)
                throw new Exception("Il y a plus de recherche que de CrossPoint...");
            
            contentOutput[indexResearch] = NeighboorsToContent(neighboors);
            
            NextResearch();
        }

        private void NextResearch()
        {
            indexResearch += 1;
            
            if (indexResearch < crossPoints.Length)
            {
                crossPoints[indexResearch].SearchNeighboors();
            }
            else // toutes les recherches ont été faites
            {
                Ouput();
                Debug.Log("Maintenance terminé");
            }
        }
        
        private string NeighboorsToContent(List<CrossPoint> neighboors)
        {
            int nCrossPoint = crossPoints.Length;
            string res = $"{crossPoints[indexResearch].name} : ";
            
            foreach (CrossPoint neighboor in neighboors)
            {
                int i;
                for (i = 0; i < nCrossPoint && neighboor != crossPoints[i]; i++)
                {}

                if (i == nCrossPoint)
                    throw new Exception("Un crossPoint n'est pas dans le crossManager");

                res += $",{i}";
            }

            return res;
        }

        // ------------ Parsing ------------
        private void Ouput()
        {
            using (StreamWriter sw = File.CreateText(Path))
            {
                foreach (string ligne in contentOutput)
                {
                    sw.WriteLine(ligne);
                }
            }
        }

        private void LoadNeighboors()
        {
            if (!File.Exists(Path))
            {
                throw new Exception("Le fichier de sauvegarde des crossPoints n'existe pas --> faire une maintenance");
            }
            
            using (StreamReader sr = File.OpenText(Path))
            {
                string ligne;
                int nCrossPoint = crossPoints.Length;

                int iCrossPoint;
                for (iCrossPoint = 0; iCrossPoint < nCrossPoint && (ligne = sr.ReadLine()) != null; iCrossPoint++)
                {
                    string[] infos = ligne.Split(',');

                    string nameCrossPoint = infos[0].Substring(0, infos[0].Length - 3);
                    
                    if (crossPoints[iCrossPoint].name != nameCrossPoint)
                        Debug.Log("WARNING : Le fichier de sauvegarde des crossPoints n'est pas compatible avec les crossPoint --> faire une maintenance");

                    // set les neighboors du cross point
                    int nInfo = infos.Length;
                    for (int i = 1; i < nInfo; i++)
                    {
                        crossPoints[iCrossPoint].AddNeighboor(crossPoints[int.Parse(infos[i])]);
                    }
                }

                if (iCrossPoint < nCrossPoint)
                {
                    Debug.Log("WARNING : Le fichier de sauvegarde des crossPoints n'est pas compatible avec les crossPoint --> faire une maintenance");
                }
            }
        }
    }
}