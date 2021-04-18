using Script.EntityPlayer;
using UnityEngine;

namespace Script.DossierPoint
{
    public class Point : MonoBehaviour
    {
        [SerializeField] private GameObject graphics;
    
        private void Start()
        {
            graphics.SetActive(false);
        }
    }
}

