using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    [SerializeField] private Menu[] menus;

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

    public void CloseMenu(string menuName)
    {
        int l = menus.Length;
        for (int i = 0; i < l; i++)
        {
            if (menus[i].menuName == menuName)
                menus[i].Close();
        }
    }
    
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
