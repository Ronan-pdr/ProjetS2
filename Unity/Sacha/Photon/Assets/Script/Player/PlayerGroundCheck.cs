using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject) // Le cas où c'est avec notre propre personnage
            return;
        
        playerController.SetGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        
        playerController.SetGroundedState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        
        playerController.SetGroundedState(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        
        playerController.SetGroundedState(true);
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        
        playerController.SetGroundedState(false);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        
        playerController.SetGroundedState(true);
    }
}
