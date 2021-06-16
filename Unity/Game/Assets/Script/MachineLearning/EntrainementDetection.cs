using Script.Tools;

namespace Script.MachineLearning
{
    public class EntrainementDetection : Entrainement
    {
        // ------------ Attributs ------------
        
        public const string NameDirectory = "SauvegardeNeuroneDetection";

        private const int CoefScore = 10;
        
        // ------------ Getter ------------
        public override string GetNameDirectory() => NameDirectory;
        
        // ------------ Public Methods ------------

        public override void EndTest()
        {
            // récupérer le score
            
            // bonus
            float dist = Calcul.Distance(Student.transform.position, begin.position);
            Score += (int)(dist * CoefScore);
            
            // le donner au classement
            Student.SetNeurone(Classement.EndEpreuve(Student.NeuralNetwork, Score));

            // recommencer
            StartEntrainement();
        }

        public override void Malus()
        {
            throw new System.NotImplementedException();
        }
        
        // ------------ Protected Methods ------------

        protected override Student GetPrefab() => Master.GetOriginalDetecteur();

        protected override void ResetIndicator()
        {}
    }
}