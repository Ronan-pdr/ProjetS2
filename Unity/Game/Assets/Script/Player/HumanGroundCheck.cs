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

    /*private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == human.gameObject)
            return;
        
        human.SetGroundedState(true);
    }*/

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == human.gameObject) // Le cas où c'est avec notre propre personnage
            return;
        
        human.SetGroundedState(true);
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject == human.gameObject)
            return;
        
        human.SetGroundedState(false);
    }

    /*private void OnCollisionStay(Collision other)
    {
        if (other.gameObject == human.gameObject)
            return;
        
        human.SetGroundedState(true);
    }*/
}
