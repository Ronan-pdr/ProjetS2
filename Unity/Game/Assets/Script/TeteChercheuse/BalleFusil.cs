using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Script;
using Script.Tools;

public class BalleFusil : TeteChercheuse
{
    private ArmeInfo armeInfo;

    public static void Tirer(Vector3 coord, GameObject ownObj, Vector3 rotation, ArmeInfo _armeInfo, float speed)
    {
        BalleFusil balleFusil = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TeteChercheuse", "Balle" + _armeInfo.GetName()), coord, Quaternion.identity).GetComponent<BalleFusil>();
        
        balleFusil.VecteurCollision(ownObj, rotation, _armeInfo, speed);
    }

    public void VecteurCollision(GameObject ownObj, Vector3 rotation, ArmeInfo _armeInfo, float speed)
    {
        SetRbTr();
        
        Find = false;
        Lanceur = ownObj;
        moveAmount = new Vector3(0, 0, speed);
        armeInfo = _armeInfo;
        
        Tr.Rotate(rotation);
    }

    public void Update()
    {
        if (!PhotonNetwork.IsMasterClient) //Seul le masterClient controle les balles
            return;
        
        if (Find || Calcul.Distance(Lanceur.transform.position, Tr.position) > armeInfo.GetPortéeAttaque()) // Fin de course : soit touché, soit max distance
        {
            if (Find && HittenObj.GetComponent<Humanoide>())
            {
                Humanoide cibleHumaine = HittenObj.GetComponent<Humanoide>();

                if (!(cibleHumaine is Chasseur)) //Si la personne touchée est un chasseur, personne prend de dégât
                {
                    cibleHumaine.TakeDamage(armeInfo.GetDamage()); //Le chassé ou le bot prend des dégâts

                    if (cibleHumaine is BotClass)
                    {
                        Lanceur.GetComponent<Chasseur>().TakeDamage(10); // Le chasseur en prend aussi puisqu'il s'est trompé de cible
                    }
                }
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
