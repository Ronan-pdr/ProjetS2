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
                case 4:
                    n.Chasseur = 1;
                    n.Chassé = 3;
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
                    n.Rectiligne = 10;
                    n.Fuyard = 1;
                    n.Suiveur = 1;
                    break;
                case 2:
                    n.Rectiligne = 0;
                    n.Fuyard = 0;
                    n.Suiveur = 0;
                    break;
                case 3:
                    n.Rectiligne = 60;
                    break;
                case 4:
                    n.Rectiligne = 34;
                    n.Fuyard = 4;
                    n.Suiveur = 2;
                    break;
                default:
                    n.Rectiligne = 50;
                    n.Fuyard = 1;
                    n.Suiveur = 1;
                    break;
            }
            
            return n;
        }
    }
}