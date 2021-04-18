using System;

namespace Script.DossierPoint
{
    public class SpawnPoint : Point
    {
        // attributs
        public enum Type
        {
            Chasseur,
            Chassé
        }

        private Type _type;
        public Type Typo => _type;

        /*private bool _pris;
        public bool Pris
        {
            get => _pris;
            set
            {
                if (!value)
                {
                    throw new Exception("Un spawn pris ne peut pas être 'dépris'");
                }

                _pris = true;
            }
        }*/
        

        // constructeur
        private void Awake()
        {
            if (name.Contains("Chasseur"))
            {
                _type = Type.Chasseur;
            }
            else if (name.Contains("Chassé"))
            {
                _type = Type.Chasseur;
            }
            else
            {
                throw new Exception($"Le nom '{name}' ne correspond à aucun spawnPoint");
            }
        }
    }
}