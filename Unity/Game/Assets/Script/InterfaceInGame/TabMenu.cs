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
        
        // ------------ Event ------------

        private void OnEnable()
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
                string n = player.name;
                if (n.Length > 9)
                {
                    n = player.name.Substring(0, 8) + n[n.Length - 1];
                }
                
                nameP.text += n + Environment.NewLine;
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
