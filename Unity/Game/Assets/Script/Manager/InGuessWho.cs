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
            IsMultijoueur = true;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            NtypeJoueur n = new NtypeJoueur();
            
            n.Chasseur = MasterManager.Instance.SettingsGame.NChasseur;
            n.Chassé = NJoueur - n.Chasseur;

            return n;
        }
        
        protected override NtypeBot GetNBot()
        {
            NtypeBot n = new NtypeBot();
            switch (NJoueur)
            {
                case 1:
                    n.Rectiligne = 6;
                    n.Fuyard = 3;
                    n.Suiveur = 1;
                    break;
                case 2:
                    n.Rectiligne = 16;
                    n.Fuyard = 3;
                    n.Suiveur = 1;
                    break;
                case 3:
                    n.Rectiligne = 26;
                    n.Fuyard = 3;
                    n.Suiveur = 1;
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