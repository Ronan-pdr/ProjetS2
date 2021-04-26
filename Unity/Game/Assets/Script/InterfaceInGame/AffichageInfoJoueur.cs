using System;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Script.EntityPlayer;
using UnityEngine.UI;

namespace Script.InterfaceInGame
{
    public class AffichageInfoJoueur : MonoBehaviourPunCallbacks
    {
        // ------------ SerializedField ------------
        [Header("Texte nom/vie")]
        [SerializeField] private TMP_Text text;

        [Header("Image vie")]
        [SerializeField] private Transform SpriteContent;
        [SerializeField] private Sprite[] sprites;
        
        // ------------ Attributs ------------
        
        private PlayerClass player;

        // ------------ Constructeurs ------------
        public void SetUp(PlayerClass value, int largeur, int hauteur)
        {
            player = value;
            
            text.transform.position = new Vector3(largeur, hauteur + 85f, 0);
            SpriteContent.transform.position = new Vector3(largeur - 45, hauteur + 80f, 0);
        }
        
        // ------------ Méthodes ------------
        private void Update()
        {
            if (player is null)
                return;
            
            UpdateImageVie();
            UpadteText();
        }

        private void UpdateImageVie()
        {
            int v = player.GetCurrentHealth();
            int maxV = player.GetMaxHealth();
            int len = sprites.Length;
                     
            Image image = SpriteContent.GetComponent<Image>();
            
            if (v < 0)
            {
                image.sprite = sprites[0];
            }
            else
            {
               image.sprite = sprites[v * (len - 1) / maxV]; 
            }
        }

        private void UpadteText()
        {
            int vie = player.GetCurrentHealth();
            text.text = vie <= 0 ? "Dead" : vie + " / " + player.GetMaxHealth(); // la vie
        }
    }
}