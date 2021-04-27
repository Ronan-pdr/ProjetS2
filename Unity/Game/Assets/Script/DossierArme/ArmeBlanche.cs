using Script.Animation;
using UnityEngine;

namespace Script.DossierArme
{
    public class ArmeBlanche : Arme
    {
        public override void UtiliserArme()
        {
            Anim.Set(HumanAnim.Type.Hit);
        }
    }
}

