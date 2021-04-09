using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Test
{
    public class TestRayGaz : Entity
    {
        [SerializeField] private GameObject destination;

        private void Start()
        {
            List<Vector3> path = RayGaz.GetPath(gameObject, destination);

            foreach (Vector3 p in path)
            {
                CreatePointPath(p);
            }
        }

        public static void CreateMarqueur(Vector3 position)
        {
            GameObject g = Instantiate(MasterManager.Instance.marqueur, position, Quaternion.identity);
            g.transform.parent = MasterManager.Instance.GetDossierBodyChercheur();
        }
        
        public static void CreatePointPath(Vector3 position)
        {
            GameObject g = Instantiate(MasterManager.Instance.PointPath, position + Vector3.up * 1f, Quaternion.identity);
            g.transform.parent = MasterManager.Instance.GetDossierBodyChercheur();
        }
    }
}