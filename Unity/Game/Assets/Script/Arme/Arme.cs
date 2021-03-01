using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public abstract class Arme : MonoBehaviour
{
    // les gameObject de la caméra et du joueur qui porte le flingue
    [SerializeField] protected Transform cameraHoder;
    [SerializeField] protected GameObject controller;
    
    // Variables relatives 
    [SerializeField] protected ArmeInfo armeInfo;
    public GameObject armeObject;
    

    public abstract void Use();
}
