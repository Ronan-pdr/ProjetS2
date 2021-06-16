using System;
using Photon.Pun;
using Script.Animation;
using Script.Bot;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Script.DossierArme;
using Script.InterfaceInGame;
using Script.Manager;
using Script.Tools;

namespace Script.EntityPlayer
{
    public class Chasseur : PlayerClass
    {
        // ------------ Serialize Field ------------
        
        [Header("Liste des armes")]
        [SerializeField] private Arme[] armes;
        
        // ------------ Attributs ------------

        private int armeIndex;
    
        // ------------ Constructeurs ------------
        
        protected override void AwakePlayer()
        {
            // Le ranger dans la liste du MasterManager
            MasterManager.Instance.AjoutChasseur(this);
        }

        protected override void StartPlayer()
        {
            MaxHealth = 100;
            etat = Etat.Debout;
            armeIndex = -1;
            EquipItem(0);
        }
        
        // ------------ Upadte ------------
        
        protected override void UpdatePlayer()
        {
            if (etat == Etat.Assis)
            {
                throw new Exception("Un chasseur ne peut-être assis");
            }
            
            ManipulerArme();
        }
    
        // ------------ Méthodes ------------

        private void ManipulerArme()
        {
            // changer d'arme avec les numéros
            for (int i = 0; i < armes.Length; i++)
            {
                if (Input.GetKey((i + 1).ToString()))
                {
                    EquipItem(i);
                    break;
                }
            }

            // changer d'arme avec la molette
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                EquipItem(SimpleMath.Mod(armeIndex + 1, armes.Length));
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                EquipItem(SimpleMath.Mod(armeIndex - 1, armes.Length));
            }

            //tirer
            if (Input.GetMouseButton(0))
            {
                armes[armeIndex].Use();
            }

            if (armes[armeIndex] is Gun)
            {
                // viser
                if (Input.GetMouseButtonDown(1))
                {
                    // commencer à viser
                    Anim.Set(HumanAnim.Type.Aiming);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    // arrêter de viser
                    Anim.Stop(HumanAnim.Type.Aiming);
                }
                
                if (Input.GetMouseButtonUp(0))
                {
                    // arrêter de tirer
                    Anim.Stop(HumanAnim.Type.Shoot);
                }
            }
        }
        
        public void EquipItem(int index) // index supposé valide
        {
            if (etat == Etat.Accroupi)
                return; // il ne peut pas changer d'arme losqu'il est accoupi

            // Le cas où on essaye de prendre l'arme qu'on a déjà
            if (index == armeIndex)
                return;
            
            // C'est le cas où on avait déjà une arme, il faut la désactiver
            if (armeIndex != -1)
            {
                armes[armeIndex].gameObject.SetActive(false);
            }
            
            armeIndex = index;
            armes[armeIndex].gameObject.SetActive(true);
            
            Anim = armes[armeIndex].Anim;

            if (Pv.IsMine)
            {
                // mettre ou enlever la visée
                master.SetVisée(armes[armeIndex] is Gun);
                
                // MULTIJOUEUR
                Hashtable hash = new Hashtable();
                hash.Add("itemIndex", armeIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        public void WhenWeaponHit(GameObject hittenObj, int armeDamage)
        {
            // Si c'est pas un humain on s'en fout
            if (hittenObj.GetComponent<Humanoide>())
            {
                Humanoide cibleHumaine = hittenObj.GetComponent<Humanoide>();
    
                if (!(cibleHumaine is Chasseur)) // Si la personne touchée est un chasseur, personne prend de dégât
                {
                    cibleHumaine.TakeDamage(armeDamage); // Le chassé ou le bot prend des dégâts
    
                    if (cibleHumaine is BotClass)
                    {
                        TakeDamage(6); // Le chasseur en prend aussi puisqu'il s'est trompé de cible
                    }
                }
            }
        }
    }
}
