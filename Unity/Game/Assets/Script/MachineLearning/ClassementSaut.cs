using System;
using TMPro;
using UnityEngine;

namespace Script.MachineLearning
{
    public class ClassementSaut : MonoBehaviour
    {
        // ------------ Attributs ------------

        [Header("Canvas")]
        [SerializeField] private GameObject menuTab;
        [SerializeField] private TextMeshProUGUI[] zonesTexte;
        
        // ------------ Attributs ------------

        private EntrainementSaut[] _zoneEntrainement;
        private int _nZone;
        
        private (NeuralNetwork Neurones, int score)[] _classement;

        // ------------ Constructeur ------------

        private void Awake()
        {
            _zoneEntrainement = GetComponentsInChildren<EntrainementSaut>();
            _nZone = _zoneEntrainement.Length;
        }

        private void Start()
        {
            _classement = new (NeuralNetwork Neurones, int score)[_nZone];

            for (int i = 0; i < _nZone; i++)
            {
                _classement[i].Neurones = _zoneEntrainement[i].Bot.Neurones;
            }
            
            AfficherClassement();
        }
        
        // ------------ Update ------------

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // afficher le classement
                menuTab.SetActive(true);
                AfficherClassement();
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                menuTab.SetActive(false);
            }
        }

        // ------------ Methods ------------

        private void AfficherClassement()
        {
            int l = zonesTexte.Length;
            int scorePerLine = _nZone / l + 1;

            for ((int i, int j) = (0, 0); i < l; i++)
            {
                for (int k = scorePerLine; k > 0 && j < _nZone; k--, j++)
                {
                    zonesTexte[j].text += $"{j}. {_classement[i].score}" + Environment.NewLine;
                }
            }
        }
    }
}