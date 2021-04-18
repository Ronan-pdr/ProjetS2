using Script.Bot;

namespace Script.Labyrinthe
{
    public class Guide : BotClass
    {
        // ------------ Constructeurs ------------
        private void Awake()
        {
            AwakeBot();
        }

        public void Start()
        {
            rotationSpeed = 600;
            
            // tout le monde le fait pour qu'il soit parenter
            StartBot();
        }
        
        // ------------ Méthodes ------------
        protected override void FiniDeTourner()
        {} // lorsqu'il a fini de tourner, il ne fait rien de plus

        protected override void Die()
        {
            throw new System.NotImplementedException();
        }
    }
}