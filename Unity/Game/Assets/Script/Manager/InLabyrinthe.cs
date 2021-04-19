using Script.DossierPoint;

namespace Script.Manager
{
    public class InLabyrinthe : ManagerGame
    {
        // ------------ Constructeur ------------
        public InLabyrinthe(int nJoueur)
        {
            NJoueur = nJoueur;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            NtypeJoueur n = new NtypeJoueur();
            n.Blocard = NJoueur;

            return n;
        }
        
        protected override NtypeBot GetNBot()
        {
            NtypeBot n = new NtypeBot();
            n.Guide = CrossManager.Instance.GetNumberPoint() / NJoueur;
            
            return n;
        }
    }
}