using Photon.Pun;
using Script;
using UnityEngine;

namespace Script
{
    // Nous n'avons pas envie que tous les ordinateurs des joueurs suivent ce script.
    // Mais, comme les body checheur s'instancie localement (pas avec photon),
    // nul besoin de rajouter des conditions.
    public class CollisionBody : Entity
    {
        [SerializeField] private TeteChercheuse teteChercheuse;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrêt pas
                return;
        
            teteChercheuse.SetFind(true);
            teteChercheuse.SetHittenObj(other.gameObject);
        }
    
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrêt pas
                return;
        
            teteChercheuse.SetFind(true);
            teteChercheuse.SetHittenObj(other.gameObject);
        }
    }
}