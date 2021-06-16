using System;
using System.IO;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Script.MachineLearning
{
    public class ClassementDarwin : MonoBehaviour
    {
        // ------------ Attributs ------------

        [Header("Canvas")]
        [SerializeField] private GameObject menuTab;
        [SerializeField] private TextMeshProUGUI[] zonesTexte;

        [Header("Sauvegarde")]
        [SerializeField] private bool mustRecoverSave;
        
        // ------------ Attributs ------------

        private EntrainementSaut[] _zoneEntrainement;
        private int _nZone;
        
        private (NeuralNetwork Neurones, int score)[] _classement;

        private Random _rnd;

        private const string NameDirectory = "SauvegardeNeuroneSaut";

        // ------------ Constructeur ------------

        private void Awake()
        {
            _zoneEntrainement = GetComponentsInChildren<EntrainementSaut>();
            _nZone = _zoneEntrainement.Length;
            
            foreach (EntrainementSaut zone in _zoneEntrainement)
            {
                zone.SetClassement(this);
            }

            _rnd = new Random();
        }

        private void Start()
        {
            _classement = new (NeuralNetwork Neurones, int score)[_nZone];

            int i = 0;
            
            if (mustRecoverSave)
            {
                string path = $"Build/{NameDirectory}/"; 
                
                for (; i < _nZone && File.Exists(path + i); i++)
                {
                    _classement[i].Neurones = NeuralNetwork.Restore(path + i);
                    _zoneEntrainement[i].Bot.SetNeurone(_classement[i].Neurones);
                }
            }
            
            for (; i < _nZone; i++)
            { 
                _classement[i].Neurones = _zoneEntrainement[i].Bot.NeuralNetwork;
            }

            menuTab.SetActive(false);
        }
        
        // ------------ Update ------------

        private void Update()
        {
            // le menu tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // afficher le classement
                menuTab.SetActive(true);
                UpdateAffichageClassement();
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                menuTab.SetActive(false);
            }
            
            // la sauvegarde
            if (Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("Sauvegarde !!!!!!");
                
                string path = $"Build/{NameDirectory}";
                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                
                for (int i = 0; i < _nZone; i++)
                {
                    _classement[i].Neurones.Save($"{path}/{i}");
                }
            }
        }
        
        // ------------ Public Methods ------------

        public NeuralNetwork EndEpreuve(NeuralNetwork neurones, int score)
        {
            int i = _nZone - 1;
            
            if (score > _classement[i].score)
            {
                // le réseau neurones est assez bon pour rentrer dans le classement
                
                for (; i > 0 && score > _classement[i - 1].score; i--)
                {
                    // décaler tous les neurones inférieurs
                    _classement[i] = _classement[i - 1];
                }

                // insérer le nouveau réseau neurones
                _classement[i] = (neurones, score);
                
                UpdateAffichageClassement();
            }
            
            // faire la somme des scores
            int sum = 0;
            for (i = 0; i < _nZone; i++)
            {
                sum += _classement[i].score;
            }

            NeuralNetwork neuralNetwork = SelectNeuralNetwork(sum);

            switch (_rnd.Next(3))
            {
                case 0:
                    // le prendre tel quel
                    return new NeuralNetwork(neuralNetwork, false);
                case 1:
                    // le muter
                    return new NeuralNetwork(neuralNetwork, true);
                default:
                    // enfanter
                    NeuralNetwork newNeurones = new NeuralNetwork(neuralNetwork, false);
                    newNeurones.Crossover(SelectNeuralNetwork(sum));
                    return newNeurones;
            }
        }
        
        // ------------ Private Methods ------------

        private void UpdateAffichageClassement()
        {
            int l = zonesTexte.Length;
            int scorePerLine = _nZone / l + 1;

            for ((int i, int j) = (0, 0); i < l; i++)
            {
                zonesTexte[i].text = "";
                
                for (int k = scorePerLine; k > 0 && j < _nZone; k--, j++)
                {
                    zonesTexte[i].text += $"{j}. {_classement[j].score}" + Environment.NewLine;
                }
            }
        }
        
        private NeuralNetwork SelectNeuralNetwork(double fitnessSum)
        {
            if (fitnessSum == 0)
            {
                return _classement[0].Neurones;
            }
            
            int r = _rnd.Next((int)fitnessSum);
            long s = 0;

            for (int i = 0; i < _nZone; i++)
            {
                s += _classement[i].score;
                
                if (r < s)
                {
                    return _classement[i].Neurones;
                }
            }

            throw new Exception($"fitnessSum = {fitnessSum} ; r = {r} ; s = {s}");
        }
    }
}