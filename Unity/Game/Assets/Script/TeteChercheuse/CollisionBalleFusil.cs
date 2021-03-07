using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class CollisionBalleFusil : Entity
{
    [SerializeField] private TeteChercheuse teteChercheuse;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == teteChercheuse.GetLanceur() || other.GetComponent<TeteChercheuse>()) // Le cas où c'est avec notre propre personnage
            return;
        
        teteChercheuse.SetFind(true);
        teteChercheuse.SetHittenObj(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == teteChercheuse.GetLanceur() || other.GetComponent<TeteChercheuse>()) // Le cas où c'est avec notre propre personnage
            return;
        
        teteChercheuse.SetFind(true);
        teteChercheuse.SetHittenObj(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == teteChercheuse.GetLanceur() || other.gameObject.GetComponent<TeteChercheuse>()) // Le cas où c'est avec notre propre personnage
            return;

        teteChercheuse.SetFind(true);
        teteChercheuse.SetHittenObj(other.gameObject);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject == teteChercheuse.GetLanceur() || other.gameObject.GetComponent<TeteChercheuse>()) // Le cas où c'est avec notre propre personnage
            return;

        teteChercheuse.SetFind(true);
        teteChercheuse.SetHittenObj(other.gameObject);
    }
}
