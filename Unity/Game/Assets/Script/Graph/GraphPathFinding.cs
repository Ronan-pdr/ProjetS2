using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.Manager;
using Script.Test;
using Script.Tools;
using UnityEngine;

namespace Script.Graph
{
    public class GraphPathFinding : MonoBehaviour
    {
        // ------------ Attributs ------------

        private string _key;
        private MyFile<CrossPoint> _file;
        private Vector3 _start;
        private CrossPoint _destination;
        private Action<List<Vector3>> _renvoi;
        
        // ------------ Getter ------------

        public string Key => _key;

        public CrossPoint Destination => _destination;
        
        // ------------ Constructeur ------------

        private void Constructeur(CrossPoint start, CrossPoint destination, string key, Action<List<Vector3>> renvoi)
        {
            _key = key;
            
            _file = new MyFile<CrossPoint>();
            start.Origin(key);
            _file.Enfiler(start);

            _start = start.transform.position;
            _destination = destination;
            _renvoi = renvoi;
        }
        
        // ------------ Static Method(s) ------------

        public static void GetPath(CrossPoint start, CrossPoint destination, string key, Action<List<Vector3>> renvoi)
        {
            GraphPathFinding graph = Instantiate(MasterManager.Instance.GetOriginalGraphPathFinding(),
                Vector3.zero, Quaternion.identity).GetComponent<GraphPathFinding>();
            
            graph.Constructeur(start, destination, key, renvoi);
            graph.InvokeRepeating(nameof(Research), 0, 0.1f);
        }
        
        // ------------ Public Method(s) ------------

        public void AddCrossPoint(CrossPoint crossPoint)
        {
            _file.Enfiler(crossPoint);
        }
        
        // ------------ Private Method(s) ------------

        private void Research()
        {
            for (int i = 0; i < 10 && !_file.IsEmpty(); i++)
            {
                _file.Defiler().SearchPath(this);
            }

            if (_file.IsEmpty())
            {
                // fin de la recherche
                List<Vector3> path = Destination.EndResearchPath(_key);

                if (SimpleMath.IsEncadré(path[path.Count - 1], _start))
                {
                    // tout va bien
                    _renvoi(path);
                }
                else
                {
                    // erreur
                    TestRayGaz.CreateMarqueur(path[path.Count - 1], TestRayGaz.Couleur.Red);
                    throw new Exception();
                }
                
                Destroy(gameObject);
            }
        }
    }
}