using Script.Manager;
using UnityEngine;

namespace Script.MachineLearning
{
    public abstract class Entrainement : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Depart")]
        [SerializeField] protected Transform begin;
        
        // ------------ Attributs ------------

        protected MasterManager Master;
        protected Student Student;
        protected int Score;
        protected ClassementDarwin Classement;

        // ------------ Getter ------------

        public Student Bot => Student;
        
        // ------------ Setter ------------

        public void SetClassement(ClassementDarwin value)
        {
            Classement = value;
        }
        
        // ------------ Constructeur ------------

        private void Start()
        {
            Master = MasterManager.Instance;
            
            Student = Instantiate(GetPrefab(), Vector3.zero, begin.rotation);

            Student.SetEntrainement(this);
            StartEntrainement();
        }
        
        // ------------ Abstract Methods ------------

        public abstract void EndTest();

        public abstract void Malus();

        protected abstract Student GetPrefab();
        
        protected abstract void ResetIndicator();

        public abstract string GetNameDirectory();
        
        // ------------ Private Methods ------------
        
        protected void StartEntrainement()
        {
            // téléportation
            Student.transform.position = begin.position;
            
            // reset score et indicateurs
            Score = 0;
            ResetIndicator();
            
            // set le bot
            Student.SetToTest();
        }
    }
}