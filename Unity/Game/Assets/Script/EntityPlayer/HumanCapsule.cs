using UnityEngine;

namespace Script.EntityPlayer
{
    public class HumanCapsule
    {
        // attributs
        private float _hauteur;
        private float _rayon;
        
        // constructeurs
        public HumanCapsule(CapsuleCollider cap)
        {
            float scale = cap.transform.localScale.y;
            _hauteur = cap.height * scale;
            _rayon = cap.radius * scale;
        }
        
        // méthodes
        public bool CanIPass(Vector3 position, Vector3 direction, float maxDistance)
        {
            // haut du corps (vers les yeux)
            Vector3 hautDuCorps = position + Vector3.up * (_hauteur - _rayon) / 2;
            
            // bas du corps (vers le haut des pieds)
            Vector3 basDuCorps = position + Vector3.up * (_rayon - _hauteur) / 2;
            
            if (Physics.CapsuleCast(hautDuCorps, basDuCorps, _rayon, direction, maxDistance))
            {
                return false;
            }

            return true;
        }
    }
}