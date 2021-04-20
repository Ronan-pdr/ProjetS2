using System;

namespace Script.DossierPoint
{
    public class SpawnPoint : Point
    {
        // ------------ Attributs ------------
        public enum Type
        {
            Chasseur,
            Chassé
        }

        private Type _type;
        public Type Typo => _type;
        
        // ------------ Getters ------------

        public bool IsChasseurSpawn() => name.Contains("Chasseur");

        public bool IsChasséSpawn() => name.Contains("Chassé");
        

        // ------------ Constructeur ------------
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