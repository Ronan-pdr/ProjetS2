using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Script.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;
    
        [SerializeField] private Menu[] menus;
        [SerializeField] private GameObject background;
        
        //--------------Pour le crédit------------
        public void jouerlavideo(VideoPlayer input)
        {
            input.Play();
            background.GetComponent<Image>().enabled = false;
        }

        public void arreterlavideo(VideoPlayer input)
        {
            input.Stop();
            background.GetComponent<Image>().enabled = true;
        }
        
        //----------------------------------------
        private void Awake()
        {
            Instance = this;
        }

        public void OpenMenu(string menuName)
        {
            int l = menus.Length;
            for (int i = 0; i < l; i++)
            {
                if (menus[i].menuName == menuName)
                {
                    menus[i].Open();
                }
                else if (menus[i].open)
                {
                    CloseMenu(menus[i]);
                }
            }
        }

        public void OpenMenu(Menu menu)
        {
            int l = menus.Length;
            for (int i = 0; i < l; i++)
            {
                if (menus[i].open)
                {
                    CloseMenu(menus[i]);
                }
            }
        
            menu.Open();
        }

        public void ForceOpenMenu(string menuName)
        {
            GetMenu(menuName).Open();
        }
    
        public void CloseMenu(Menu menu)
        {
            menu.Close();
        }

        public void CloseMenu(string menuName)
        {
            GetMenu(menuName).Close();
        }

        private Menu GetMenu(string menuName)
        {
            int i;
            int l = menus.Length;
            for (i = 0; i < l && menus[i].menuName != menuName; i++)
            {}

            if (i == l)
                throw new Exception($"Le menu {menuName} n'existe pas");

            return menus[i];
        }
    }
}
