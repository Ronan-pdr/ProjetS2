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

            // hunter
            Effacer(hunterNames, hunterLifes);
            
            int l = mastermanager.GetNbChasseur();

            for (int i = 0; i < l; i++)
            {
                Write(mastermanager.GetChasseur(i), hunterNames, hunterLifes);
            }
            
            // hunted
            Effacer(huntedNames, huntedLifes);
            
            l = mastermanager.GetNbChassé();

            for (int i = 0; i < l; i++)
            {
                Write(mastermanager.GetChassé(i), huntedNames, huntedLifes);
            }
            
            // fonction auxiliaire

            void Write(PlayerClass player, TextMeshProUGUI nameP, TextMeshProUGUI life)
            {
                nameP.text += player.name + Environment.NewLine;
                life.text += $"{player.GetCurrentHealth()}/{player.GetMaxHealth()}" + Environment.NewLine;
            }

            void Effacer(TextMeshProUGUI nameP, TextMeshProUGUI life)
            {
                nameP.text = "";
                life.text = "";
            }
        }
    }
} 
