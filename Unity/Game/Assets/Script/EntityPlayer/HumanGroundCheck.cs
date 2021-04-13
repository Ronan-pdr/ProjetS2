using System;
using System.Collections;
using System.Collections.Generic;
using Script.Bot;
using UnityEngine;

namespace Script.EntityPlayer
{
    public class HumanGroundCheck : Entity
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
        
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject == human.gameObject) // Le cas où c'est avec notre propre personnage
                return;

            human.SetGroundedState(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == human.gameObject) // Le cas où c'est avec notre propre personnage
                return;
        
            human.SetGroundedState(false);
        }
    }
}

