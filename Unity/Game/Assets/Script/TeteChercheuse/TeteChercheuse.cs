using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Script.Tools;

public class TeteChercheuse : Entity
{
    private float speed = 8f;
    
    //Variable initialiser avec les fonctions
    private bool Find;
    private GameObject Lanceur;
    private GameObject HittenObj;
    private float PortéAttaque;

    //Getter
    public GameObject GetLanceur() => Lanceur;
    
    public GameObject GetHittenObj() => HittenObj;
    
    //Setter
    public void SetFind(bool find)
    {
        Find = find;
    }
    
    public void SetHittenObj(GameObject hittenObj)
    {
        HittenObj = hittenObj;
    }

    public static TeteChercheuse Initialisation(Vector3 coord)
    {
        return PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TeteChercheuse"), coord, Quaternion.identity).GetComponent<TeteChercheuse>();
    }
    
    public void VecteurCollision(GameObject ownObj, Vector3 rotation, float portéAttaque)
    {
        AwakeEntity();
        
        Find = false;
        Lanceur = ownObj;
        PortéAttaque = portéAttaque;
        moveAmount = new Vector3(0, 0, speed);
        
        transform.Rotate(rotation);
    }

    public void Update()
    {
        if (!gameObject || !Tr) //Sans cela, le multijoueur créé des bugs
            return;
        
        if (Find || Calcul.Distance(Lanceur.transform.position, Tr.position) > PortéAttaque)
        {
            if (Find)
            {
                Debug.Log("I hit a " + HittenObj);
            }
        
            Debug.Log("I didn't hit anything");

            Chasseur chasseur = Lanceur.GetComponent<Chasseur>();
            chasseur.TakeDamage(chasseur.GetDamageAie());

            moveAmount = Vector3.zero;
            PhotonNetwork.Destroy(gameObject);
            enabled = false;
        }
    }

    public void FixedUpdate()
    {
        if (gameObject && Rb)
            moveEntity();
    }
}
