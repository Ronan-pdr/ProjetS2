using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Script.Tools;

public class TeteChercheuse : MonoBehaviour
{
    [SerializeField] private static GameObject prefabs;
    private static float vitesse = 0.1f;
    
    //Variable initialiser avec les fonctions
    private static bool Find;
    private static GameObject Lanceur;
    private static GameObject TeCheObj;
    private static GameObject HittenObj;

    //Getter
    public static GameObject GetLanceur() => Lanceur;
    
    //Setter
    public static void SetFind(bool find)
    {
        Find = find;
    }
    
    public static void SetHittenObj(GameObject hittenObj)
    {
        HittenObj = hittenObj;
    }

    private static GameObject Initialisation(Vector3 depart)
    {
        return PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TeteChercheuse"), depart, Quaternion.identity);
    }
    
    public static GameObject VecteurCollision(GameObject ownObj, Vector3 depart, Vector3 directionSens, float distanceMax)
    {
        Find = false;
        Lanceur = ownObj;
        TeCheObj = Initialisation(depart);

        while (!Find && Calcul.Distance(Lanceur.transform.position, TeCheObj.transform.position) < distanceMax)
        {
            TeCheObj.transform.position += directionSens * vitesse;
        }
        
        Destroy(TeCheObj);

        if (Find)
        {
            Debug.Log("I hit a " + HittenObj);
            return HittenObj;
        }

        return null;
    }
}
