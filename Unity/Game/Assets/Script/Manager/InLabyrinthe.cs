namespace Script.Manager
{
    public class InLabyrinthe : ManagerGame
    {
        protected override (int chassé, int chasseur) GetN(int nJoueur)
        {
            (int chassé, int chasseur) n;
            n.chasseur = 0;
            n.chassé = nJoueur;

            return n;
        }
    }
}