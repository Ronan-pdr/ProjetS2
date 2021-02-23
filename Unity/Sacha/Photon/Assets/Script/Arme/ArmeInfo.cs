using UnityEngine;


namespace Script.Player
{
    [CreateAssetMenu(menuName = "FPS/New Arme Info")]
    public class ArmeInfo : ScriptableObject
    {
        public string armeName;
        public float damage;
        public float portéeAttaque;
    }
}