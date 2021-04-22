using Script.EntityPlayer;
using Script.Manager;
using UnityEngine;

namespace Script.DossierPoint
{
    public class Point : MonoBehaviour
    {
        // ------------ Serialized Field ------------
        
        [Header("Graphics")]
        [SerializeField] private GameObject graphics;
    
        // ------------ Constructeur ------------
        private void Start()
        {
            if (this is CrossPoint && MasterManager.Instance.IsInMaintenance())
                return;
            
            graphics.SetActive(false);
        }
    }
}

