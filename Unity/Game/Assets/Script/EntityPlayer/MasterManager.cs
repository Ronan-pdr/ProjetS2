using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

namespace Script
{
    public class MasterManager : MonoBehaviour
    {
        //Cela permet d'y accéder facilement
        public static MasterManager Instance;
        
        // Est ce que la liste a déjà été instancié
        private bool DejaFait;
        private float debutJeu; // le moment exacte ou le jeu commence
        private float ecart = 1; // Au bout de combien de temps il instancie les listes
        
        // Accéder aux différents joueurs, chaque joueur sera donc stocké deux fois, sauf s'il est mort, il sera juste un spectateur
        private List<PlayerClass> players;
        private List<Chasseur> chasseurs;
        private List<Chassé> chassés;
        private List<Spectateur> spectateurs;

        // Getter
        public int GetNbPlayer() => players.Count;
        public PlayerClass GetPlayer(int index) => players[index];

        private void Awake()
        {
            Instance = this;
            
            players = new List<PlayerClass>();
            chasseurs = new List<Chasseur>();
            chassés = new List<Chassé>();
            spectateurs = new List<Spectateur>();

            debutJeu = Time.time;
        }

        private void Update()
        {
            if (DejaFait || Time.time - debutJeu < ecart)
            {
                return;
            }
            
            foreach (PlayerClass player in GetComponentsInChildren<PlayerClass>())
            {
                players.Add(player);

                if (player is Chasseur)
                {
                    chasseurs.Add((Chasseur)player);
                }
                else
                {
                    chassés.Add((Chassé)player);
                }
            }

            DejaFait = true;
        }

        public void Die(PlayerClass player, PhotonView pv)
        {
            players.Remove(player);

            if (player is Chassé)
            {
                chassés.Remove((Chassé) player);
            }
            
            Spectateur spectateur = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "Spectateur"),
                Vector3.zero, Quaternion.identity, 0, new object[]{pv.ViewID}).GetComponent<Spectateur>();
            
            spectateurs.Add(spectateur);
        }
    }
}