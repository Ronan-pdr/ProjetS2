using System;
using Photon.Pun;
using UnityEngine;
using TMPro;

namespace Script
{
    public class AffichageInfoJoueur : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text text;
        private PlayerClass player;

        public void SetUp(PlayerClass _player, int hauteur, int largeur)
        {
            player = _player;
            
            text.transform.position = new Vector3(largeur, hauteur, 0);
        }

        public void Update()
        {
            if (player == null)
                return;
            
            text.text = (player is Chassé?"Chassé":"Chasseur") + Environment.NewLine; // le type
            text.text += player.GetPlayer().NickName + Environment.NewLine; // le nom

            int vie = player.GetCurrentHealth();
            text.text += vie <= 0 ? "Dead" : vie + " / " + player.GetMaxHealth(); // la vie
        }
    }
}