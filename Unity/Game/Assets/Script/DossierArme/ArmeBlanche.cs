using System;
using Script.Animation;
using Script.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.DossierArme
{
    public class ArmeBlanche : Arme
    {
        // ------------ Attributs ------------

        private HitArmeBlanche _trigger;
        
        // ------------ Constructeur ------------

        private void Start()
        {
            _trigger = GetComponentInChildren<HitArmeBlanche>();
        }

        // ------------ Méthode ------------
        public override void UtiliserArme()
        {
            Debug.Log("Clef molette dans ta bouche");

            _trigger.HitBegin();
            Anim.Set(HumanAnim.Type.Hit);
        }
    }
}

