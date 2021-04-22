using System.Collections.Generic;
using Script.InterfaceInGame;
using UnityEngine;

namespace Script.EntityPlayer
{
    // ------------ Enum ------------
        
    public enum TypeTouche
    {
        Avancer = 0,
        Reculer = 1,
        Droite = 2,
        Gauche = 3,
        Sprint = 4,
        Jump = 5,
        Accroupi = 6,
        Assoir = 7
    }

    public class TouchesClass
    {
        private class Touche
        {
            // ------------ Attributs ------------
        
            public KeyCode Key;
            public string StrSauvegarde;
        
            // ------------ Constructeur ------------
            public Touche(string strSauvegarde, string defaultValue)
            {
                Key = (KeyCode) System.Enum.Parse(typeof(KeyCode), 
                    PlayerPrefs.GetString(strSauvegarde, defaultValue));

                StrSauvegarde = strSauvegarde;
            }
            
            // ------------ Méthodes ------------
            private bool IsNull() => Key == GetNullKeyCode();

            public override string ToString()
            {
                if (IsNull())
                {
                    return "";
                }

                return Key.ToString();
            }
        }
        
        // ------------ Attributs ------------
        public static TouchesClass Instance;

        private Dictionary<TypeTouche, Touche> dict;
        
        // ------------ Constructeurs ------------
        
        public TouchesClass()
        {
            Instance = this;

            dict = new Dictionary<TypeTouche, Touche>();
            dict.Add(TypeTouche.Avancer, new Touche("forwardKey", "Z"));
            dict.Add(TypeTouche.Reculer, new Touche("backwardKey", "S"));
            dict.Add(TypeTouche.Droite, new Touche("rightKey", "D"));
            dict.Add(TypeTouche.Gauche, new Touche("leftKey", "Q"));
            dict.Add(TypeTouche.Sprint, new Touche("sprintKey", "LeftShift"));
            dict.Add(TypeTouche.Jump, new Touche("jumpKey", "Space"));
            dict.Add(TypeTouche.Accroupi, new Touche("crouchKey", "C"));
            dict.Add(TypeTouche.Assoir, new Touche("sitKey", "X"));
        }
    
        // ------------ Getters ------------
        public KeyCode GetKey(TypeTouche typeTouche)
        {
            return dict[typeTouche].Key;
        }

        public string GetStrSauvegarde(TypeTouche typeTouche)
        {
            return dict[typeTouche].StrSauvegarde;
        }

        public List<TypeTouche> GetSameTouches(TypeTouche typeTouche, KeyCode keyCode)
        {
            List<TypeTouche> res = new List<TypeTouche>();
            
            foreach (KeyValuePair<TypeTouche, Touche> e in dict)
            {
                if (e.Value.Key == keyCode)
                {
                    res.Add(e.Key);
                }
            }

            return res;
        }
        
        public static KeyCode GetNullKeyCode() => KeyCode.Joystick1Button19;
        
        // ------------ Setters ------------
        public void SetKey(TypeTouche typeTouche, KeyCode keyCode)
        {
            dict[typeTouche].Key = keyCode;
        }
        
        // ------------ Méthodes ------------
        public string ToString(TypeTouche typeTouche)
        {
            return dict[typeTouche].ToString();
        }
    }
}
