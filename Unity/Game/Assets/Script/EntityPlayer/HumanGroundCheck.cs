using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanGroundCheck : MonoBehaviour
{
    private Humanoide human;

    private void Awake()
    {
        human = GetComponentInParent<Humanoide>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == human.gameObject) // Le cas où c'est avec notre propre personnage
            return;
        
        human.SetGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == human.gameObject)
            return;
        
        human.SetGroundedState(false);
    }

    //Comme ça fait que l'on peut sauter plusieurs fois, je mets avant lequel l'event OnTriggerStay puisse fonctionner
    private float ecartTime = 0.5f;
    private float previousTime;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == human.gameObject)
            return;

        if (Time.time - previousTime > ecartTime)
        {
            previousTime = Time.time;
            human.SetGroundedState(true);
        }
    }
}