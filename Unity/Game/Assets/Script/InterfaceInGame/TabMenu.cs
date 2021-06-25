using System;
using Script.EntityPlayer;
using Script.Manager;
using TMPro;
using UnityEngine;

namespace Script.InterfaceInGame
{
    public class TabMenu : MonoBehaviour
    {
        // ------------ SerializedField ------------

        [Header("Texte Hunter")]
        [SerializeField] private TextMeshProUGUI hunterNames;
        [SerializeField] private TextMeshProUGUI hunterLifes;
        
        [Header("Texte Hunted")]
        [SerializeField] private TextMeshProUGUI huntedNames;
        [SerializeField] private TextMeshProUGUI huntedLifes;
        
    
        // ------------ Attributs ------------
    
        public static TabMenu Instance;

        // ------------ Constructeur ------------

        private void Start()
        {
            Set();
        }

        // ------------ Publique méthodes ------------

        public void Set()
        {
            MasterManager mastermanager = MasterManager.Instance;

            for (int i = 0; i < mastermanager.GetNbChasseur(); i++)
            {
                Write(mastermanager.GetChasseur(i), hunterNames, hunterLifes);
            }

            for (int i = 0; i < mastermanager.GetNbChassé(); i++)
            {
                Write(mastermanager.GetChassé(i), huntedNames, huntedLifes);
            }

            void Write(PlayerClass player, TextMeshProUGUI nameP, TextMeshProUGUI life)
            {
                nameP.text = player.name + Environment.NewLine;
                life.text = $"{player.GetCurrentHealth()}/{player.GetMaxHealth()}" + Environment.NewLine;
            }
        }
    }
} 
