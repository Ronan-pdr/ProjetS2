using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Test
{
    public class TestRayGaz : Entity
    {
        // ------------ SerializeField ------------
        
        [SerializeField] private Transform depart;
        [SerializeField] private Transform destination;

        // ------------ Attributs ------------
        
        public enum Couleur
        {
            Red,
            Yellow,
            Brown
        }

        private float timeBegin;
        
        // ------------ Constructeur ------------

        private void Start()
        {
            //RayGaz.GetPath(depart.position, destination.position, RecepRayGaz);
            timeBegin = Time.time + 2;
        }

        private void Update()
        {
            if (Time.time < timeBegin)
                return;

            enabled = false;
            RayGaz.GetPath(depart.position, destination.position, RecepRayGaz);
        }

        // ------------ Méthodes ------------
        
        public void RecepRayGaz(List<Vector3> path)
        {
            foreach (Vector3 p in path)
            {
                CreateMarqueur(p + Vector3.up * 1.2f, Couleur.Red);
            }
        }

        public static GameObject CreateMarqueur(Vector3 position, Couleur couleur, float scale = 1)
        {
            GameObject prefab;
            switch (couleur)
            {
                case Couleur.Yellow:
                    prefab = MasterManager.Instance.marqueurYellow;
                    break;
                case Couleur.Red:
                    prefab = MasterManager.Instance.marqueurRed;
                    break;
                case Couleur.Brown:
                    prefab = MasterManager.Instance.marqueurBrown;
                    break;
                default:
                    throw new Exception();
            }
            
            GameObject g = Instantiate(prefab, position, Quaternion.identity);
            g.transform.parent = MasterManager.Instance.GetDossierRayGaz();
            g.transform.localScale = new Vector3(scale, scale, scale);

            return g;
        }
    }
}