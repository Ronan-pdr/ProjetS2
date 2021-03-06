using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Script
{
    public class MasterManager : MonoBehaviour
    {
        //Cela permet d'y accéder facilement
        public static MasterManager Instance;
        
        // Est ce que la liste a déjà été instancié
        private bool DejaFait;
        private float debutJeu; // le moment exacte ou le jeu commence
        private float ecart = 0.3f; // Au bout de combien de temps il instancie les listes
        
        // Accéder aux différents joueurs, chaque joueur sera donc stocké deux fois, sauf s'il est mort, il sera juste un spectateur
        private List<PlayerClass> players;
        private List<Chasseur> chasseurs;
        private List<Chassé> chassés;
        private List<Spectateur> spectateurs;
        
        // Accéder facilement à ton avatar
        private PlayerClass ownPlayer;

        // Getter
        public int GetNbPlayer() => players.Count;
        public List<PlayerClass> GetListPlayer() => players;
        public PlayerClass GetPlayer(int index) => players[index];
        public PlayerClass GetOwnPlayer() => ownPlayer;
        
        //Setter
        public void SetOwnPlayer(PlayerClass _ownPlayer)
        {
            ownPlayer = _ownPlayer;
        }

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
            if (DejaFait || Time.time - debutJeu < ecart) // attend que tout le monde est déplacé son joueur dans le MasterManager
                return;
            
            foreach (PlayerClass player in GetComponentsInChildren<PlayerClass>()) // Prend tous les PlayerClass classé pour les ranger
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
            
            // Donner le feu vert à tous les script qui attendaient les listes
            InterfaceInGame.Instance.FeuVert();
        }

        public void Die(Player player)
        {
            int i;
            for (i = 0; i < players.Count && !players[i].GetPlayer().Equals(player); i++) //cherche le joueur dans la liste des players
            {}

            PlayerClass playerClass = players[i]; // S'il y a un problème d'index à cette ligne c'est que tu tentes de supprimer un joueur de la liste qui n'y est plus
            
            players.RemoveAt(i); // remove de la liste players

            if (playerClass is Chassé)
            {
                chassés.Remove((Chassé) playerClass); // remove de la liste chassés (si c'était un chassé)
            }
            else
            {
                chasseurs.Remove((Chasseur) playerClass); // remove de la liste chasseurs (si c'était un chasseur)
            }

            PhotonView pv = playerClass.GetPV(); // on récupère le point de vue du mourant

            if (!pv.IsMine) // Seul le mourant envoie le hash
                return;
            
            Debug.Log("envoie du hash");
            
            Spectateur spectateur = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "Spectateur"),
                Vector3.zero, Quaternion.identity, 0, new object[]{pv.ViewID}).GetComponent<Spectateur>(); // création du spectateur
            
            spectateurs.Add(spectateur); // ajout à la liste spectateurs
            
            Hashtable hash = new Hashtable();
            hash.Add("MortPlayer", player);
            player.SetCustomProperties(hash); // envoie du hash pour que les autres le supprime de leur liste
        }
    }
}