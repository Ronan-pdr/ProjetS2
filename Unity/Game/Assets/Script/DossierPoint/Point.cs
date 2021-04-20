using Script.EntityPlayer;
using Script.Manager;
using UnityEngine;

namespace Script.DossierPoint
{
    public class Point : MonoBehaviour
    {
        [SerializeField] private GameObject graphics;
    
        private void Start()
        {
            if (this is CrossPoint && MasterManager.Instance.IsInMaintenance())
                return;
            
            graphics.SetActive(false);
        }
    }
}

