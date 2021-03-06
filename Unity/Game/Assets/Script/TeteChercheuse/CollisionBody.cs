using Photon.Pun;
using Script;
using UnityEngine;

namespace Script
{
    public class CollisionBody : Entity
    {
        [SerializeField] private TeteChercheuse teteChercheuse;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) // Seul le masterClient contrôle les tetes chercheuses
                return;
            
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrêt pas
                return;
        
            teteChercheuse.SetFind(true);
            teteChercheuse.SetHittenObj(other.gameObject);
        }
    
        private void OnCollisionEnter(Collision other)
        {
            if (!PhotonNetwork.IsMasterClient) // Seul le masterClient contrôle les tetes chercheuses
                return;
            
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrêt pas
                return;
        
            teteChercheuse.SetFind(true);
            teteChercheuse.SetHittenObj(other.gameObject);
        }
    }
}