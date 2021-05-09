using System;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Graph;
using Script.TeteChercheuse;
using Script.Tools;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Script.DossierPoint
{
    public class CrossPoint : Point
    {
        // ------------ SerializeField ------------
        
        [Header("Ajustement")]
        [SerializeField] private List<CrossPoint> neighboors;

        // ------------ Attributs ------------
        
        // Les instances (uniques)
        private CrossManager _crossManager;
        private CrossMaintenance _crossMaintenance;
        
        // pour la maintenance
        private MyFile<CrossPoint> _potentialNeighboors;
        private int _indexFile;
        
        // ------------ Getters ------------
        public CrossPoint GetNeighboor(int index) => neighboors[index];
        public int GetNbNeighboor() => neighboors.Count;
        public int IndexFile => _indexFile;

        // ------------ Setter ------------
        public void AddNeighboor(CrossPoint value)
        {
            Line.Create(transform.position, value.transform.position);
            
            neighboors.Add(value);
        }
        
        // ------------ Constructeur ------------
        private void Awake()
        {
            neighboors = new List<CrossPoint>();
        }

        private void Start()
        {
            _crossManager = CrossManager.Instance;

            if (!CrossManager.Instance.IsMaintenance)
            {
                // On veut pas les voir
                Invisible();
            }
        }
        
        // ------------ Méthodes ------------

        public static int NameToIndex(string namePoint)
        {
            string s = "";
            for (int i = namePoint.Length - 2; i >= 0 && namePoint[i] != '('; i--)
            {
                s = namePoint[i] + s;
            }

            if (int.TryParse(s, out int index))
            {
                return index;
            }
            
            throw new Exception($"Le nom '{namePoint}' n'est pas homologé (index = {s})");
        }
        
        // ------------ Maintenance ------------

        private void Error()
        {
            if (!CrossManager.Instance.IsMaintenance)
            {
                throw new Exception("Impossible que cette fonction soit appelé si on n'est pas en maintenance");
            }
        }
        
        public void EndResearchBody(CrossPoint neighboor) // est appelé dans la class 'BodyChercheur', dans la fonction 'Update'
        {
            Error();
            
            if (_potentialNeighboors is null) // tout reçu
            {
                throw new Exception("trop de résultat de body chercheur ont été reçu");
            }

            // est-ce un voisin valide
            if (!(neighboor is null))
            {
                // N'était il pas déjà dedans ?
                if (neighboors.Contains(neighboor))
                {
                    throw new Exception($"Ca devrait être impossible (Lanceur -> {name}, Dest -> {neighboor})");
                }
                
                neighboors.Add(neighboor);
                _crossMaintenance.OneNewNeighboorFind(); // incrémente un indicateur
            }

            if (_potentialNeighboors.IsEmpty()) // tout reçu
            {
                _potentialNeighboors = null;
                _crossMaintenance.EndOfPointResearch(this, neighboors);
            }
            else // ça continue d'en envoyer
            {
                NextResearch();
            }
        }

        public void SearchNeighboors(CrossMaintenance crossMaintenance, int indexFile)
        {
            _indexFile = indexFile;
            _crossMaintenance = crossMaintenance;
            
            if (!_crossManager)
            {
                _crossManager = CrossManager.Instance;
            }
            
            Error();

            _potentialNeighboors = GetPotentialNeigboors();

            if (_potentialNeighboors.IsEmpty()) // aucun voisin potentiel donc c'est la fin de la recherche
            {
                _crossMaintenance.EndOfPointResearch(this, neighboors);
            }
            else // ça commence
            {
                NextResearch();
            }
        }

        private void NextResearch()
        {
            Error();
            
            GameObject destination = _potentialNeighboors.Defiler().gameObject;
            BodyRectilgne.InstancierStatic(gameObject, destination);
        }
        
        private MyFile<CrossPoint> GetPotentialNeigboors()
        {
            Error();
            
            MyFile<CrossPoint> potentialNeighboors = new MyFile<CrossPoint>();
            Vector3 ownCoord = transform.position;
            float distanceMax = 22;
            int n = 0;
            
            //Debug.Log(name);

            foreach (CrossPoint crossPoint in _crossManager.GetCrossPoints())
            {
                // déjà repertorié
                if (neighboors.Contains(crossPoint))
                {
                    //Debug.Log($"Déja trouvé -> {potentialNeighboors[i].name}");
                    continue;
                }

                Vector3 pos = crossPoint.transform.position;
                float distanceThisWithDest = Calcul.Distance(ownCoord, pos);
                
                // trop proche ou trop loin
                if (!(0.4f < distanceThisWithDest && distanceThisWithDest < distanceMax))
                { 
                    //Debug.Log(potentialNeighboors[i].name);
                    continue;
                }

                float diffAlt = Calcul.Distance(ownCoord.y, pos.y);
                    
                // trop de d'altitude pour la potentiel distance de montée
                if (Calcul.Distance(ownCoord, pos, Calcul.Coord.Y)*0.7f <= diffAlt)
                {
                    //Debug.Log(potentialNeighboors[i].name);
                    continue; 
                }
                
                // plus de deux étages de différences...
                if (diffAlt > 9)
                {
                    continue;
                }
                
                potentialNeighboors.Enfiler(crossPoint);
                n++;
            }
            
            //Debug.Log($"{name} va envoyé {n} bodyChercheur(s)");

            return potentialNeighboors;
        }
    }
}