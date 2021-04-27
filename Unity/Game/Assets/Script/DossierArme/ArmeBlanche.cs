using Script.Animation;
using UnityEngine;

namespace Script.DossierArme
{
    public class ArmeBlanche : Arme
    {
        public override void UtiliserArme()
        {
            Debug.Log("Clef molette dans ta bouche");
            Anim.Set(HumanAnim.Type.Hit);
        }
    }
}

