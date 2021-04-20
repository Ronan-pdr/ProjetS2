using System;
using Script.Bot;
using Script.DossierPoint;
using Script.EntityPlayer;

namespace Script.Manager
{

    public abstract class ManagerGame
    {
        // ------------ Struct ------------
        protected struct NtypeJoueur
        {
            public int Chassé;
            public int Chasseur;
            public int Blocard;
            public int None;
        }
        
        protected struct NtypeBot
        {
            public int Rectiligne;
            public int Fuyard;
            public int Guide;
        }
        
        // ------------ Attributs ------------
        
        protected int NJoueur;

        // ------------ Getters ------------
        protected abstract NtypeJoueur GetNJoueur();
        protected abstract NtypeBot GetNBot();

        // Donne une liste indiquant le type de chaque joueur en fonction 
        public TypePlayer[] GetTypePlayer()
        {
            // récupérer les taux en fonction du type de la partie
            NtypeJoueur n = GetNJoueur();

            CasEreur();

            TypePlayer[] res = new TypePlayer[NJoueur];

            // attribution des rôles (pour l'instant c'est pas random)
            int i;
            for (i = 0; i < n.Chasseur; i++)
            {
                res[i] = TypePlayer.Chasseur;
            }
            
            for (int j = 0; j < n.Chassé; i++, j++)
            {
                res[i] = TypePlayer.Chassé;
            }
            
            for (int j = 0; j < n.Blocard; i++, j++)
            {
                res[i] = TypePlayer.Blocard;
            }

            for (int j = 0; j < n.None; i++, j++)
            {
                res[i] = TypePlayer.None;
            }

            return res;

            void CasEreur()
            {
                if (NJoueur == 0)
                {
                    throw new Exception("Il ne peut y avoir 0 joueur");
                }
                
                // le total doît toujours être égal au nombre de joueur (logique hehe)
                if (n.Chasseur + n.Chassé + n.None + n.Blocard != NJoueur)
                {
                    throw new Exception($"{n.Chasseur} + {n.Chassé} + {n.None} + {n.Blocard} != {NJoueur}");
                }
                
                // avec le nombre de spawn
                SpawnManager spawnManager = SpawnManager.Instance;
                if (n.Chasseur > spawnManager.GetNbSpawnChasseur())
                {
                    throw new Exception("Pas assez de spawn pour les chasseurs");
                }

                if (n.Chassé > spawnManager.GetNbSpawnChassé())
                {
                    throw new Exception("Pas assez de spawn pour les chassés");
                }
            }
        }

        public TypeBot[] GetTypeBot()
        {
            // récupérer les taux en fonction du type de la partie
            NtypeBot n = GetNBot();

            TypeBot[] res = new TypeBot[n.Fuyard + n.Rectiligne + n.Guide];

            // attribution des types (pour l'instant c'est pas random)
            int i;
            for (i = 0; i < n.Fuyard; i++)
            {
                res[i] = TypeBot.Fuyard;
            }
            
            for (int j = 0; j < n.Rectiligne; i++, j++)
            {
                res[i] = TypeBot.Rectiligne;
            }

            for (int j = 0; j < n.Guide; i++, j++)
            {
                res[i] = TypeBot.Guide;
            }

            return res;
        }
    }
}