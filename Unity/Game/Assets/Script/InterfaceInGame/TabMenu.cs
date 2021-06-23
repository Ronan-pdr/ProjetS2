using Script.EntityPlayer;
using Script.Manager;
using UnityEngine;

namespace Script.InterfaceInGame
{
    public class TabMenu : MonoBehaviour
    {
        // ------------ SerializedField ------------
    
        [Header("Prefab")]
        [SerializeField] Transform ChasseurInfoContent;
        [SerializeField] Transform ChasseInfoContent;
        [SerializeField] PlayerInfoTab playerInfoTabPrefab;
    
        // ------------ Attributs ------------
    
        public static TabMenu Instance;

        // ------------ Constructeur ------------
    
        public void Set()
        {
            MasterManager mastermanager = MasterManager.Instance;
            PlayerInfoTab infos;
        
            for (int i = 0; i < mastermanager.GetNbChasseur(); i++)
            {
                PlayerClass playerclass = mastermanager.GetChasseur(i);
            
                infos = Instantiate(playerInfoTabPrefab, ChasseurInfoContent).GetComponent<PlayerInfoTab>();
                infos.Set(playerclass);
            }
        
            for (int i = 0; i < mastermanager.GetNbChassé(); i++)
            {
                PlayerClass playerclass = mastermanager.GetChassé(i);
            
                infos = Instantiate(playerInfoTabPrefab, ChasseInfoContent).GetComponent<PlayerInfoTab>();
                infos.Set(playerclass);
            }
        }
    }
} 
