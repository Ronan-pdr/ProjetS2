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

    private PhotonView PV;

    private void Start()
    {
        SetRbTr();

        PV = GetComponent<PhotonView>();

        // parenter
        Tr.parent = MasterManager.Instance.GetDossierBalleFusil();
        
        Find = false;
        moveAmount = new Vector3(0, 0, 50);
    }

    public static void Tirer(Vector3 coord, GameObject ownObj, Vector3 rotation, ArmeInfo _armeInfo)
    {
        BalleFusil balleFusil = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TeteChercheuse", "Balle" + _armeInfo.GetName()), coord, Quaternion.identity).GetComponent<BalleFusil>();
        
        balleFusil.VecteurCollision(ownObj, rotation, _armeInfo);
    }

    public void VecteurCollision(GameObject ownObj, Vector3 rotation, ArmeInfo _armeInfo)
    {
        Lanceur = ownObj;
        armeInfo = _armeInfo;
        
        transform.Rotate(rotation);
    }

    public void Update()
    {
        if (!PV.IsMine) //Seul le masterClient controle les balles
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
                        Lanceur.GetComponent<Chasseur>().TakeDamage(1); // Le chasseur en prend aussi puisqu'il s'est trompé de cible
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
