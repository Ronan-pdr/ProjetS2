using System;

namespace Script.Manager
{
    public class InMaintenance : ManagerGame
    {
        // ------------ Constructeur ------------
        public InMaintenance(int nJoueur)
        {
            NJoueur = nJoueur;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            if (NJoueur > 1)
            {
                throw new Exception($"Il ne peut y avoir plus d'un joueur, il y en a {NJoueur}");
            }
            
            NtypeJoueur n = new NtypeJoueur();
            n.None = 1;

            return n;
        }

        protected override NtypeBot GetNBot()
        {
            // 0 bot
            return new NtypeBot();
        }
    }
}