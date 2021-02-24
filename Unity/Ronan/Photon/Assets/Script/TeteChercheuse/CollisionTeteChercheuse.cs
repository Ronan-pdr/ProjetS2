using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTeteChercheuse : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == TeteChercheuse.GetLanceur()) // Le cas où c'est avec notre propre personnage
            return;

        TeteChercheuse.SetFind(true);
        TeteChercheuse.SetHittenObj(other.gameObject);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == TeteChercheuse.GetLanceur())
            return;
        
        TeteChercheuse.SetFind(true);
        TeteChercheuse.SetHittenObj(other.gameObject);
    }
}
