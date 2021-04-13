using Script.EntityPlayer;
using UnityEngine;

namespace Script.DossierPoint
{
    public class Point : MonoBehaviour
    {
        [SerializeField] private GameObject graphics;
    
        private void Awake()
        {
            graphics.SetActive(false);
        }
    }
}

