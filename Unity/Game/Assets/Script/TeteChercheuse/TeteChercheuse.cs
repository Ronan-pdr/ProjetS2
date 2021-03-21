using UnityEngine;
using Script.EntityPlayer;

namespace Script.TeteChercheuse
{
    public abstract class TeteChercheuse : Entity
    {
        // Cette classe mère va regrouper tous les objets lancés depuis une personne
        // pour vérifier les collisions et pottentiellement récupérer les objets touchés
        
        protected bool Find;
        protected GameObject Lanceur; // C'est un chasseur, je ne fais pas la conversion pour les collisions
        protected GameObject HittenObj;
        
        //Getter
        public GameObject GetLanceur() => Lanceur;
    
        public GameObject GetHittenObj() => HittenObj;
    
        //Setter
        public void SetFind(bool find)
        {
            Find = find;
        }
    
        public void SetHittenObj(GameObject hittenObj)
        {
            HittenObj = hittenObj;
        }
    }
}