using System;
using Script.EntityPlayer;

namespace Script.Manager
{
    public class InGuessWho : ManagerGame
    {
        protected override (int chassé, int chasseur) GetN(int nJoueur)
        {
            (int chassé, int chasseur) n;
            switch (nJoueur)
            {
                case 1:
                    n.chasseur = 1;
                    n.chassé = 0;
                    break;
                default:
                    n.chasseur = 1;
                    n.chassé = nJoueur - 1;
                    break;
            }

            return n;
        }
    }
}