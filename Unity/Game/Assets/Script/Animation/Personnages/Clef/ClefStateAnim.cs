using System.Collections.Generic;
using UnityEngine;

namespace Script.Animation.Personnages.Clef
{
    public class ClefStateAnim : HumanAnim
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
            dict.Add(Type.Hit, Animator.StringToHash("Hit"));

            return dict;
        }

        // Update is called once per frame
        /*void Update()
    {
        animator.SetBool(HitHash,false);
        // --------------- Action --------------------------

        bool jumpEnded = Input.GetKeyDown("right shift"); //To change when merging !
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isSquatting = animator.GetBool(isSquattingHash);
        
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWkngBack = animator.GetBool(isWkngBackHash);
        bool isSideWalkingL = animator.GetBool(isSWLHash);
        bool isSideWalkingR = animator.GetBool(isSWRHash);
        bool isLF = animator.GetBool(isLFHash);
        bool isRF = animator.GetBool(isRFHash);
        bool isHit = animator.GetBool(HitHash);

        // --------------- Key detection -------------------

        //Clicking Needed
        bool Attack = Input.GetMouseButtonDown(0);

        // - Shooting
        animator.SetBool(HitHash, Attack);
    }*/
    }
}
