using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Script.Tools;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Script.DossierPoint
{
    public class CrossMaintenance : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Maintenance")]
        [SerializeField] private bool InMaintenance;
        [SerializeField] private SousCrossManager _sousCrossManager;
        
        // ------------ Attributs ------------

        public static CrossMaintenance Instance;

        // servira pour l'output
        private string[] contentOutput;

        private int _nResultAttendu;
        private int nNewNeighboor;
        
        // calculer la complexité
        private Stopwatch time;

        // Instance
        private CrossManager _crossManager;
        
        // ------------ Getters ------------
        
        public bool IsMaintenance => InMaintenance;
        
        // ------------ Setters ------------

        public void OneNewNeighboorFind()
        {
            nNewNeighboor += 1;
        }
        
        // ------------ Constructeurs ------------

        private void Awake()
        {
            if (InMaintenance)
            {
                Instance = this;
            }
            
            // si on n'est pas en maintenance, cette class ne sert à rien
        }

        private void Start()
        {
            if (InMaintenance)
            {
                _crossManager = CrossManager.Instance;
                BeginMaintenance();
            }
            
            // si on n'est pas en maintenance, cette class ne sert à rien
        }

        // ------------ Méthodes ------------
        
        private void BeginMaintenance()
        {
            _crossManager.LoadNeigboors();

            _nResultAttendu = _sousCrossManager.NCrossPoint;

            if (_nResultAttendu == 0)
            {
                throw new Exception($"Le sousCrossManager '{_sousCrossManager.name}' ne contient aucun crossPoint");
            }

            for (int i = 0; i < _nResultAttendu; i++)
            {
                _sousCrossManager.CrossPoints[i].SearchNeighboors(i);
            }

            contentOutput = new string[_nResultAttendu];
                
            // enregistrer temps au début de la recherche
            time = new Stopwatch();
            time.Start();
        }
        
        public void EndOfPointResearch(CrossPoint lanceur, List<CrossPoint> neighboors)
        {
            if (_nResultAttendu <= 0)
            {
                throw new Exception("Il y a plus de recherche que de CrossPoint...");
            }

            contentOutput[lanceur.IndexFile] = NeighboorsToContent(lanceur.name, neighboors);
            _nResultAttendu -= 1;
            
            if (_nResultAttendu == 0) // toutes les recherches ont été faites
            {
                Ouput();
                time.Stop();
                Debug.Log($"Maintenance de '{_sousCrossManager.name}' est terminé et a trouvé {nNewNeighboor} nouveaux voisins");
                Debug.Log($"La maintence s'est effectué en {time.ElapsedMilliseconds/60000} minutes et {time.ElapsedMilliseconds%60000/1000} secondes");
            }
        }
        
        private string NeighboorsToContent(string pointName, List<CrossPoint> neighboors)
        {
            string res = $"{pointName} : ";
            
            foreach (CrossPoint neighboor in neighboors)
            {
                // Chercher l'index du voisin
                res += $",{CrossPoint.NameToIndex(neighboor.name)}";
            }

            return res;
        }
        
        // ------------ Parsing ------------
        private void Ouput()
        {
            string path = _crossManager.GetPath();
            
            // Créer le dossier s'il n'existe pas
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter sw = File.CreateText(path + _sousCrossManager.name))
            {
                foreach (string ligne in contentOutput)
                {
                    sw.WriteLine(ligne);
                }
            }
        }
    }
}