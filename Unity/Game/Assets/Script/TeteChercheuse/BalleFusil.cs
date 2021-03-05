using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Script;
using Script.Tools;

public class BalleFusil : Entity
{
    private bool Find;
    private GameObject Lanceur; // C'est un chasseur, je ne fais pas la conversion pour les collisions
    private GameObject HittenObj;
    
    private ArmeInfo armeInfo;

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

    public static void Tirer(Vector3 coord, GameObject ownObj, Vector3 rotation, ArmeInfo _armeInfo, float speed)
    {
        BalleFusil balleFusil = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Balle" + _armeInfo.GetName()), coord, Quaternion.identity).GetComponent<BalleFusil>();
        
        balleFusil.VecteurCollision(ownObj, rotation, _armeInfo, speed);
    }

    public void VecteurCollision(GameObject ownObj, Vector3 rotation, ArmeInfo _armeInfo, float speed)
    {
        SetRbTr();
        
        Find = false;
        Lanceur = ownObj;
        moveAmount = new Vector3(0, 0, speed);
        armeInfo = _armeInfo;
        
        transform.Rotate(rotation);
    }

    public void Update()
    {
        if (!PhotonNetwork.IsMasterClient) //Seul le masterClient controle les balles
            return;
        
        if (Find || Calcul.Distance(Lanceur.transform.position, Tr.position) > armeInfo.GetPortéeAttaque()) // Fin de course : soit touché, soit max distance
        {
            if (Find && HittenObj.GetComponent<Humanoide>())
            {
                Debug.Log("I hit a " + HittenObj);
                
                Humanoide cibleHumaine = HittenObj.GetComponent<Humanoide>();

                if (!(cibleHumaine is Chasseur)) //Si la personne touchée est un chasseur, personne prend de dégât
                {
                    cibleHumaine.TakeDamage(armeInfo.GetDamage()); //Le chassé ou le bot prend des dégâts

                    if (cibleHumaine is BotClass)
                    {
                        Lanceur.GetComponent<Chasseur>().TakeDamage(armeInfo.GetDamage()); //Le chasseur en prend aussi puisqu'il s'est trompé de cible
                    }
                }
            }
            else
            {
                Debug.Log("I didn't hit a human");
            }

            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) //Seul le masterClient controle les balles
            return;
        
        moveEntity();
    }
}
