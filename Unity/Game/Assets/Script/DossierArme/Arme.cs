using UnityEngine;

namespace Script.DossierArme
{
    public abstract class Arme : MonoBehaviour
    {
        // les gameObject de la caméra et du joueur qui porte le flingue
        [SerializeField] protected Transform cameraHolder;
        [SerializeField] protected GameObject controller;
    
        // Variables relatives à l'arme en elle-même
        [SerializeField] protected ArmeInfo armeInfo;
        public GameObject armeObject;
    
        // fréquence de tir
        private float lastUse = -1;


        public void Use()
        {
            if (Time.time - lastUse > armeInfo.GetPériodeAttaque())
            {
                UtiliserArme();
                lastUse = Time.time;
            }
        }
    
        public abstract void UtiliserArme();
    }
}

