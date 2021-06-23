using System;
using Script.EntityPlayer;
using Script.Tools;
using TMPro;
using UnityEngine;

namespace Script.InterfaceInGame
{
    public class PlayerInfoTab : MonoBehaviour
    {
        // ------------ SerializedField ------------

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI textName;
        [SerializeField] private TextMeshProUGUI textLife;
        
        // ------------ Attributs ------------
    
        // relatif au player
        private PlayerClass _player;
        private string _playerName;
    
        // autre
        private const int Limitation = 15;

        // ------------ Constructeurs ------------

        public void Set(PlayerClass value)
        {
            _player = value;
            _playerName = _player.name;
        
            if (_playerName.Length >= Limitation)
            {
                // couper les lettres en trop
                _playerName = ManString.Cut(_playerName, 0, Limitation);
            }
        }
    
        // ------------ Upadte ------------
    
        void Update()
        {
            if (_player is null)
                return;

            // texte
            textName.text = _playerName;
        
            // vie
            int vie = _player.GetCurrentHealth();
            textLife.text = vie <= 0 ? "Dead" : $"{vie} / {_player.GetMaxHealth()}" + Environment.NewLine;
        }
    }
}