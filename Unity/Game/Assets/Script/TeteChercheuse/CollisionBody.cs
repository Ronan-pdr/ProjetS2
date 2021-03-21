using System;
using UnityEngine;
using Script.EntityPlayer;
using Script.Tools;

namespace Script.TeteChercheuse
{
    // Nous n'avons pas envie que tous les ordinateurs des joueurs suivent ce script.
    // Mais, comme les body checheur s'instancie localement (pas avec photon),
    // nul besoin de rajouter des conditions.
    public class CollisionBody : Entity
    {
        [SerializeField] private TeteChercheuse teteChercheuse;
        [SerializeField] private CapsuleCollider _collider;

        private float GetYSol()
        {
            float y = _collider.transform.position.y;

            return y;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrête pas
                return;

            _collider.isTrigger = false;
        }
    
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrête pas
            {
                _collider.isTrigger = true;
                return;
            }

            ContactPoint[] listContact = other.contacts;
            int len = listContact.Length;
            
            int i;
            for (i = 0; i < len && Calcul.Distance(listContact[i].point.y, GetYSol()) < _collider.radius; i++)
            {}

            if (i < len) // cela signifie qu'un objet (qui n'est pas entity) l'a touché à une hauteur supérieur au rayon 
            {
                teteChercheuse.SetFind(true);
                teteChercheuse.SetHittenObj(other.gameObject);
            }
        }
        
        
        
        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrête pas
            {
                _collider.isTrigger = true;
            }
        }
    }
}