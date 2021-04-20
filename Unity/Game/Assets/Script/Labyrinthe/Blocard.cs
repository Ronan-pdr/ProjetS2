using System;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Test;
using Script.Tools;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class Blocard : PlayerClass
    {
        // ------------ Attributs ------------
        private int nCaillouMax = 50;
        private int nCaillou;
        private List<GameObject> caillous = new List<GameObject>();

        // ------------ Constructeurs ------------
        private void Awake()
        {
            AwakePlayer();
        }

        void Start()
        {
            MaxHealth = 100;
            StartPlayer();

            nCaillou = 0;
        }

        // méthodes
        private void Update()
        {
            if (!Pv.IsMine)
                return;
            
            UpdatePlayer();
            
            if (Input.GetKeyDown("f"))
            {
                PoserCaillou();
            }

            if (Input.GetKey("r"))
            {
                RetirerCaillou();
            }
            
            Animation();
        }

        private void FixedUpdate()
        {
            FixedUpdatePlayer();
        }

        private void PoserCaillou()
        {
            caillous.Add(TestRayGaz.CreateMarqueur(Tr.position));
            nCaillou += 1;
                
            if (nCaillou == nCaillouMax)
            {
                SupprimerCaillou(0);
            }
        }

        private void RetirerCaillou()
        {
            for (int i = caillous.Count - 1; i >= 0; i--)
            {
                if (SimpleMath.IsEncadré(caillous[i].transform.position + Vector3.down * 0.8f, Tr.position))
                {
                    SupprimerCaillou(i);
                }
            }
        }

        private void SupprimerCaillou(int i)
        {
            Destroy(caillous[i]);
            caillous.RemoveAt(i);
            nCaillou -= 1;
        }
        
        protected override void Die()
        {
            throw new System.NotImplementedException();
        }
    }
}