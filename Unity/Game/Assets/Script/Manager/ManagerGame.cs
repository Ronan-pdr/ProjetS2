using System;

namespace Script.Manager
{
    public abstract class ManagerGame
    {
        // Enum pour la création des joueurs
        public enum TypePlayer
        {
            Chasseur = 0,
            Chassé = 1,
            Spectateur = 2
        }
        
        protected abstract (int chassé, int chasseur) GetN(int nJoueur);

        // Donne une liste indiquant le type de chaque joueur en fonction 
        public TypePlayer[] GetTypePlayer(int nJoueur)
        {
            (int chassé, int chasseur) n = GetN(nJoueur);
            
            if (n.chasseur + n.chassé != nJoueur)
            {
                throw new Exception($"{n.chasseur} + {n.chassé} = {nJoueur}");
            }

            TypePlayer[] res = new TypePlayer[nJoueur];

            // attribution des rôles (pour l'instant c'est pas random)
            for (int i = 0; i < n.chasseur; i++)
            {
                res[i] = TypePlayer.Chasseur;
            }
            
            for (int i = n.chasseur; i < n.chassé; i++)
            {
                res[i] = TypePlayer.Chassé;
            }

            return res;
        }
    }
}