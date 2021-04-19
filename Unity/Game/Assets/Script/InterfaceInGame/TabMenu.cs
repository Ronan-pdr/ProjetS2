using System.Collections;
using System.Collections.Generic;
using Script.EntityPlayer;
using TMPro;
using UnityEngine;

public class TabMenu : MonoBehaviour
{
    public static TabMenu Instance;
    [SerializeField] Transform ChasseurInfoContent;
    [SerializeField] Transform ChasseInfoContent;
    [SerializeField] GameObject playerInfoTabPrefab;
    private List<PlayerInfoTab> infosChasseur;
    private List<PlayerInfoTab> infosChasse;

    void Awake()
    {
           
    }

    public void Set()
    {
        infosChasseur = new List<PlayerInfoTab>();
        infosChasse = new List<PlayerInfoTab>(); 
        MasterManager mastermanager = MasterManager.Instance;
        for (int i = 0; i < mastermanager.GetNbChasseur(); i++)
        {
            PlayerClass playerclass = mastermanager.GetChasseur(i);
            infosChasseur.Add(Instantiate(playerInfoTabPrefab, ChasseurInfoContent).GetComponent<PlayerInfoTab>());
            Debug.Log(infosChasseur[i]);
            infosChasseur[i].Set(playerclass);
        }
        
        for (int i = 0; i < mastermanager.GetNbChassé(); i++)
        {
            PlayerClass playerclass = mastermanager.GetChassé(i);
            infosChasse.Add(Instantiate(playerInfoTabPrefab, ChasseInfoContent).GetComponent<PlayerInfoTab>());
            infosChasse[i].Set(playerclass);
        }
    }
    
} 
