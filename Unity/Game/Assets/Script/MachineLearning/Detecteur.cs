using System;
using System.IO;
using Script.Animation;
using Script.Tools;
using UnityEngine;

namespace Script.MachineLearning
{
    public class Detecteur : Student
    {
        // ------------ Attributs ------------

        private NeuralNetwork _knowToJump;
        private bool _sitting;
        private MyFile<Collider> _fileWall;

        private bool _isWaitingResult;

        // ------------ Setter ------------

        public override void SetToTest()
        {
            SetStandUp();
            running = Running.Marche;
        }

        private void SetSit()
        {
            if (!_sitting)
            {
                Anim.Set(HumanAnim.Type.Sit);
            }
            
            _sitting = true;
        }

        private void SetStandUp()
        {
            if (_sitting)
            {
                Anim.Stop(HumanAnim.Type.Sit);
            }
            
            _sitting = false;
        }
        
        // ------------ Constructeur ------------

        protected override void AwakeStudent()
        {
            string path = $"Build/{EntrainementSaut.NameDirectory}/0";
            if (File.Exists(path))
            {
                _knowToJump = NeuralNetwork.Restore(path);
            }
            else
            {
                throw new Exception($"Path = '{path}'");
            }
            
            _fileWall = new MyFile<Collider>();
            _isWaitingResult = false;
        }

        // ------------ Methods ------------

        protected override void ErrorEntrainement()
        {
            if (!(Entrainement is EntrainementDetection))
            {
                throw new Exception();
            }
        }
        
        // ------------ Brain ------------

        protected override int[] GetLayerDimension()
        {
            // 1 entrée :
            // - la hauteur du premier obstacle
            
            // 1 sortie :
            // - est-ce un obstacle infranchissable ?

            return new[] {1, 1};
        }
        
        protected override void UseBrain()
        {
            if (!Grounded || _isWaitingResult)
                return;
            
            // recupérer les infos par rapport aux obstacles
            (double dist, double height) = GetDistHeightFirstObstacle(Tr.position, MaxDistJump);
            
            double[] outputJump = GetResult(_knowToJump, InputJump(dist, height));

            if (outputJump[0] > 0.5f)
            {
                // il FAUT sauter (on part du principe que notre cerveau a raison)
                Jump();
                _isWaitingResult = true;
                Invoke(nameof(Result), 1f);
                
                // input correspondant à cette tâche
                double[] input = {height};
            
                ErrorInput(input);

                // output des neurones
                double[] outputDetec = GetResult(Neurones, input);

                if (outputDetec[0] > 0.8f)
                {
                    // pense que l'on peut pas passer
                    SetSit();
                }
                else
                {
                    SetStandUp();
                }
            }
        }

        private void Result()
        {
            if (!_isWaitingResult)
                return;
            
            // il n'a rencontré aucun obstacle
            // puiqu'il attend toujours un résultat

            if (_sitting)
            {
                // a mal anticipé
                Entrainement.EndTest();
            }

            _isWaitingResult = false;
        }
        
        private void Rematerialiser()
        {
            while (!_fileWall.IsEmpty())
            {
                _fileWall.Defiler().isTrigger = false;
            }
        }
        
        // ------------ Event ------------
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                // fin de l'épreuve (tout réussi)
                Entrainement.EndTest();
            }
        }

        protected override void OnHighCollision(Collision other)
        {
            if (_sitting)
            {
                // bien anticipé --> continue l'épreuve
                other.collider.isTrigger = true;
                _fileWall.Enfiler(other.collider);
                Invoke(nameof(Rematerialiser), 0.5f);
            }
            else
            {
                // perdu
                Entrainement.EndTest();
            }

            // on a déja le résultat
            _isWaitingResult = false;
        }
    }
}