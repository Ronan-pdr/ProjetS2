﻿namespace Script.Manager
{
    public class InLabyrinthe : ManagerGame
    {
        // constructeur
        public InLabyrinthe(int nJoueur)
        {
            NJoueur = nJoueur;
        }
        
        // méthodes
        protected override NtypeJoueur GetNJoueur()
        {
            NtypeJoueur n = new NtypeJoueur();
            n.Blocard = NJoueur;

            return n;
        }
        
        protected override NtypeBot GetNBot()
        {
            // 0 bot
            return new NtypeBot();
        }
    }
}