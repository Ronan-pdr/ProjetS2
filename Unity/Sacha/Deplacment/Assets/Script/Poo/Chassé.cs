using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Script
{
    public class Chassé : Player
    {
        private static Vector3 distanceCamera = new Vector3(0, 0, 0);

        public Chassé(Transform tr, Rigidbody rg)
        {
            Tr = tr;
            Rg = rg;
        }
        
        public override void Upd()
        {
            MovementPlayer();
        }
    }
}