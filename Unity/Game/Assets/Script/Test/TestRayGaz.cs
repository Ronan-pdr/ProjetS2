using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Test
{
    public class TestRayGaz : Entity, ISReceveurRayGaz
    {
        [SerializeField] private GameObject destination;

        private void Start()
        {
            RayGaz.GetPath(gameObject.transform.position, destination.transform.position, this);
        }
        
        public void RecepRayGaz(List<Vector3> path)
        {
            foreach (Vector3 p in path)
            {
                CreatePointPath(p);
            }
        }

        public static GameObject CreateMarqueur(Vector3 position)
        {
            GameObject g = Instantiate(MasterManager.Instance.marqueur, position + Vector3.up * 0.8f, Quaternion.identity);
            g.transform.parent = MasterManager.Instance.GetDossierRayGaz();

            return g;
        }
        
        public static void CreatePointPath(Vector3 position)
        {
            GameObject g = Instantiate(MasterManager.Instance.PointPath, position + Vector3.up * 1, Quaternion.identity);
            g.transform.parent = MasterManager.Instance.GetDossierRayGaz();
        }
    }
}