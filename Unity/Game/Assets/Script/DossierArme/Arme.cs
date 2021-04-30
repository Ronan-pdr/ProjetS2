using System;
using Script.Animation;
using UnityEngine;

namespace Script.DossierArme
{
    public abstract class Arme : MonoBehaviour
    {
        // ------------ Serialize Field ------------
        
        // les gameObject de la caméra et du joueur qui porte le flingue
        [Header("Controller")]
        [SerializeField] protected Transform cameraHolder;

        // Variables relatives à l'arme en elle-même
        [Header("Info")]
        [SerializeField] protected ArmeInfo armeInfo;
        
        [Header("Animation")]
        [SerializeField] protected HumanAnim anim;
    
        // ------------ Attributs ------------
        
        // pour la fréquence de tir
        private float lastUse = -1;


        // ------------ Getter ------------
        public HumanAnim Anim => anim;

        // ------------ Méthodes ------------

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

