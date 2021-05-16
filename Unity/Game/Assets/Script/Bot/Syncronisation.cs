using System;
using System.Collections.Generic;
using Script.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Bot
{
    public class Syncronisation : MonoBehaviour
    {
        // ------------ Attributs ------------

        public static Syncronisation Instance;
        private List<Hirondelle> _hirondelles;
        private int _lenght;
        
        // ------------ Setter ------------

        public void AddHirondelle(Hirondelle value)
        {
            _hirondelles.Add(value);
            _lenght += 1;
        }
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            Instance = this;
            _hirondelles = new List<Hirondelle>();
        }

        // ------------ Update ------------

        private void Update()
        {
            for (int i = 0; i < _lenght; i++)
            {
                Hirondelle h1 = _hirondelles[i];
                
                for (int j = i + 1; j < _lenght; j++)
                {
                    Case(h1, _hirondelles[j]);
                }
            }
        }
        
        // ------------ Méthodes ------------

        private void Case(Hirondelle h1, Hirondelle h2)
        {
            Transform tr1 = h1.transform;
            Transform tr2 = h2.transform;

            Vector3 pos1 = tr1.position;
            Vector3 pos2 = tr2.position;
            
            float dist = Calcul.Distance(pos1, pos2);

            if (dist < 3)
            {
                h1.Eloigner(pos2);
                h2.Eloigner(pos1);
            }
            else if (dist < 4f)
            {
                h1.Aligner(tr1.eulerAngles.y);
                h2.Aligner(tr2.eulerAngles.y);
            }
            else if (dist < 5f)
            {
                h1.Rapprocher(pos2);
                h2.Rapprocher(pos1);
            }
        }
    }
}