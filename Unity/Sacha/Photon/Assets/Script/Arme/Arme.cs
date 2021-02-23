using System.Collections;
using System.Collections.Generic;
using Script.Player;
using UnityEngine;

public abstract class Arme : MonoBehaviour
{
    public ArmeInfo armeInfo;
    [SerializeField] protected GameObject controller;
    public GameObject armeGameObject;
    [SerializeField] protected Camera cam;

    public abstract void Use();
}
