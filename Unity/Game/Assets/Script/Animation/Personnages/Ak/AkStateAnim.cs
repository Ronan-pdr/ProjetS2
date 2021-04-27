using System.Collections;
using System.Collections.Generic;
using Script.Animation;
using UnityEngine;

public class AkStateAnim : HumanAnim
{
    // ------------ Constructeur ------------

    protected override Dictionary<Type, int> GetDict()
    {
        // l'instancier
        Dictionary<Type, int> dict = new Dictionary<Type, int>();
        
        // deplacement
        dict.Add(Type.Forward, Animator.StringToHash("isWalking"));
        dict.Add(Type.Backward, Animator.StringToHash("isWalkingBack"));
        dict.Add(Type.Right, Animator.StringToHash("isSideWalkingR"));
        dict.Add(Type.Left, Animator.StringToHash("isSideWalkingL"));
        dict.Add(Type.DiagR, Animator.StringToHash("isRF"));
        dict.Add(Type.DiagL, Animator.StringToHash("isLF"));
        dict.Add(Type.Run, Animator.StringToHash("isRunning"));
        
        // one touch
        dict.Add(Type.Sit, Animator.StringToHash("isSitting"));
        dict.Add(Type.Squat, Animator.StringToHash("isSquatting"));
        dict.Add(Type.Jump, Animator.StringToHash("isJumping"));
        
        // arme
        dict.Add(Type.Aiming, Animator.StringToHash("isAiming"));
        dict.Add(Type.Shoot, Animator.StringToHash("isShooting"));

        return dict;
    }

    // Update is called once per frame
    /*void Update()
    {
        // --------------- Action --------------------------

        bool isAiming = animator.GetBool(isAimingHash);
        bool isShooting = animator.GetBool(isShootingHash);
        bool asMunition = animator.GetBool(asMunitionHash);

        // --------------- Key detection -------------------

        //Clicking Needed
        bool Shoot = Input.GetMouseButton(0);
        bool Aim = Input.GetMouseButton(1);

        // -- Tmp
        if (Input.GetKeyDown("r"))
            animator.SetBool(asMunitionHash,true);
        if (Input.GetKeyDown("o"))
            animator.SetBool(asMunitionHash,false);

        
        

        // - Shooting
        animator.SetBool(isShootingHash, asMunition && Shoot);
        animator.SetBool(isAimingHash, Aim);
    }*/
}
