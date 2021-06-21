using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.Graph;
using UnityEngine;

namespace Script.Test
{
    public class TestGraphPathFinding : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [SerializeField] private CrossPoint depart;
        [SerializeField] private CrossPoint destination;
        
        // ------------ Constructeur ------------

        private void Start()
        {
            GraphPathFinding.GetPath(depart, destination, "sacha", Recep);
        }
        
        // ------------ Methods ------------

        private void Recep(List<Vector3> path)
        {
            foreach (Vector3 point in path)
            {
                TestRayGaz.CreateMarqueur(point, TestRayGaz.Couleur.Brown);
            }
        }
    }
}