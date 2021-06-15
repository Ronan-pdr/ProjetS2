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
        private class Node
        {
            // ------------ Attibuts ------------

            private float _bestDist;
            private Node _previous;
            private Line _bridge;
            private Vector3 _pos;
            
            // ------------ Getter ------------
            public float BestDist => _bestDist;
            public Node Previous => _previous;
            public Vector3 Pos => _pos;
            
            // ------------ Setter ------------

            public void BetterDist(float dist)
            {
                if (dist > _bestDist)
                {
                    throw new Exception();
                }

                _bestDist = dist;
            }

            public bool NewPath(Node node)
            {
                
                float dist = node._bestDist + Calcul.Distance(_pos, node._pos);
                
                if (dist < _bestDist)
                {
                    _bestDist = dist;
                    _previous = node;
                    _bridge.SetColor(dist);
                    return true;
                }
                
                return false;
            }
            
            // ------------ Constructeur ------------

            public Node(Vector3 pos)
            {
                // l'origine d'un graph
                
                _bestDist = 0;
                _previous = null;
                _bridge = null;
                _pos = pos;
            }
            
            public Node(float dist, Node previous, Vector3 pos)
            {
                if (dist < 0)
                {
                    throw new Exception("Une distance entre deux ne peut être négatifs");
                }
                
                _previous = previous;
                _pos = pos;
                
                _bestDist = dist + Calcul.Distance(_pos, previous._pos);

                if (!(_previous is null))
                {
                    Vector3 pos1 = _pos;
                    Vector3 pos2 = _previous._pos;
            
                    _bridge = Line.Create(pos1, pos2, _bestDist);
                }
            }
            
            // ------------ Method(s) ------------

            private float DistToCouleur(float dist)
            {
                return dist * 30;
            }
        }
        
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
        
        // pour le path finding
        private Dictionary<string, Node> _Nodes;

        // ------------ Getters ------------
        public CrossPoint GetNeighboor(int index) => neighboors[index];
        public int GetNbNeighboor() => neighboors.Count;
        public int IndexFile => _indexFile;

        // ------------ Setter ------------
        public void AddNeighboor(CrossPoint value)
        {
            //Line.Create(transform.position, value.transform.position);
            
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
        
        // ------------ PathFinding ------------

        public void Origin(string key)
        {
            if (_Nodes.ContainsKey(key))
            {
                throw new Exception("Impossible que l'origine d'un graph soit déjà parcouru durant cette recherche puiqu'elle est censée commencer");
            }
            
            _Nodes.Add(key, new Node(transform.position));
        }

        public void SearchPath(GraphPathFinding graph)
        {
            string key = graph.Key;
            
            if (!_Nodes.ContainsKey(key))
            {
                throw new Exception();
            }

            float dist = _Nodes[key].BestDist;
            
            foreach (CrossPoint crossPoint in neighboors)
            {
                crossPoint.Parcourir(graph, dist);
            }
        }

        private void Parcourir(GraphPathFinding graph, float dist)
        {
            string key = graph.Key;
            
            if (_Nodes.ContainsKey(key))
            {
                // ce cross point a déjà été parcouru par cette recherche
                if (_Nodes[key].NewPath(_Nodes[key]))
                {
                    // le nouveau chemin trouvé est plus court --> ajustement
                    Ajustement(key);
                }
            }
            else
            {
                // ce cross point n'a jamais été parcouru par cette recherche
                _Nodes.Add(key, new Node(dist, _Nodes[key], transform.position));
                if (this != graph.Destination)
                {
                    // si c'est la destination, inutile de relancer la recherche sur celui-ci
                    graph.AddCrossPoint(this);
                }
            }
        }

        private void Ajustement(string key)
        {
            foreach (CrossPoint crossPoint in neighboors)
            {
                if (crossPoint._Nodes.ContainsKey(key))
                {
                    // ce point a déjà été parcouru
                    
                    if (crossPoint._Nodes[key].Previous == _Nodes[key])
                    {
                        // ce point avait pour chemin ce cross point --> il faut update

                        crossPoint._Nodes[key].BetterDist(_Nodes[key].BestDist + Calcul.Distance(transform.position, crossPoint.transform.position));
                        crossPoint.Ajustement(key);
                    }
                }
            }
        }

        public List<Vector3> EndResearchPath(string key)
        {
            List<Vector3> path = new List<Vector3>();
            
            // c'est la position de la destination
            Node node;
            path.Add(transform.position);

            while (_Nodes[key].Previous != null)
            {
                node = _Nodes[key].Previous;
                path.Add(node.Pos);
            }

            return path;
        }

        // ------------ Maintenance ------------

        private void Error(string nameFunc)
        {
            if (!CrossManager.Instance.IsMaintenance)
            {
                throw new Exception($"Impossible que la fonction '{nameFunc}' soit appelé s'il n'y a pas en maintenance");
            }
        }
        
        public void EndResearchBody(CrossPoint neighboor) // est appelé dans la class 'BodyChercheur', dans la fonction 'Update'
        {
            Error("EndResearchBody");
            
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
            
            Error("SearchNeighboors");

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
            Error("NextResearch");
            
            GameObject destination = _potentialNeighboors.Defiler().gameObject;
            BodyRectilgne.InstancierStatic(gameObject, destination);
        }
        
        private MyFile<CrossPoint> GetPotentialNeigboors()
        {
            Error("GetPotentialNeigboors");
            
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
                    
                // trop d'altitude pour la potentiel distance de montée
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