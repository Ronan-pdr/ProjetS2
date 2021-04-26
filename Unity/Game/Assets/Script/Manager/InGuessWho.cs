using System;
using Script.EntityPlayer;

namespace Script.Manager
{
    public class InGuessWho : ManagerGame
    {
        // ------------ Constructeur ------------
        public InGuessWho(int nJoueur)
        {
            NJoueur = nJoueur;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            NtypeJoueur n = new NtypeJoueur();
            switch (NJoueur)
            {
                case 1:
                    n.Chasseur = 1;
                    n.Chassé = 0;
                    break;
                default:
                    n.Chasseur = 1;
                    n.Chassé = NJoueur - 1;
                    break;
            }

            return n;
        }
        
        protected override NtypeBot GetNBot()
        {
            NtypeBot n = new NtypeBot();
            switch (NJoueur)
            {
                case 1:
                    n.Rectiligne = 15;
                    n.Fuyard = 0;
                    n.Suiveur = 0;
                    break;
                default:
                    n.Rectiligne = 4;
                    n.Fuyard = 2;
                    n.Suiveur = 1;
                    break;
            }
            
            return n;
        }
    }
}