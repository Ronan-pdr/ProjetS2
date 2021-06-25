using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;
using Script.Animation.Personnages.Hunted;
using Script.Bot;
using Script.Manager;
using Random = System.Random;

namespace Script.EntityPlayer
{
    public class Chassé : PlayerClass
    {
        // ------------ SerializeField ------------

        [Header("Design")]
        [SerializeField] private GameObject[] designs;

        [Header("Position Camera")]
        [SerializeField] private Transform trCamera;
        
        // ------------ Attributs ------------

        private DesignHumanoide _design;

        // ------------ Constructeurs ------------

        protected override void AwakePlayer()
        {
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutChassé(this);
            
            Anim = GetComponent<HuntedStateAnim>();
            _design = new DesignHumanoide(Anim, designs);
        }

        protected override void StartPlayer()
        {
            MaxHealth = 100;
            master.SetVisée(true);

            if (Pv.IsMine)
            {
                ChangeDesign(new Random().Next(_design.Length));
            }
        }
        
        // ------------ Update ------------

        protected override void UpdatePlayer()
        {
            if (Input.GetMouseButton(0))
            {
                // le joueur souhaite changer de design
                TryChangeDesign();
            }
            
            // changer de design avec la molette
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                ChangeDesign((_design.Index + 1) % _design.Length);
            }
        }
        
        // ------------ Methods ------------

        private void TryChangeDesign()
        {
            Ray ray = new Ray(trCamera.position, trCamera.TransformDirection(Vector3.forward));

            if (Physics.Raycast(ray, out RaycastHit hit, 30))
            {
                if (hit.collider.GetComponent<Chassé>())
                {
                    int index = hit.collider.GetComponent<Chassé>()._design.Index;
                    ChangeDesign(index);
                }
                else if (hit.collider.GetComponent<BotClass>())
                {
                    int index = hit.collider.GetComponent<BotClass>().IndexDesign;
                    ChangeDesign(index);
                }
            }
        }

        private void ChangeDesign(int index)
        {
            if (!this)
                return;

            _design.Set(index);

            if (Pv.IsMine)
            {
                // MULTIJOUEUR
                Hashtable hash = new Hashtable();
                hash.Add("designIndex", index);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }


        // ------------ Multijoueur ------------

        protected override void PropertiesUpdate(Hashtable changedProps)
        {
            // design du chassé -> ChangeDesign (Chassé)
            if (!Pv.IsMine) // ça ne doit pas être ton point de vue puisque tu l'as déjà fait
            {
                if (changedProps.TryGetValue("designIndex", out object indexDesign))
                {
                    ChangeDesign((int)indexDesign);
                }
            }
        }
    }
}