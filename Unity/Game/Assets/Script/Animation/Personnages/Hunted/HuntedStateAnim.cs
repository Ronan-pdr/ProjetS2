using System.Collections.Generic;
using UnityEngine;

namespace Script.Animation.Personnages.Hunted
{
    public class HuntedStateAnim : HumanAnim
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

            return dict;
        }


        // Update is called once per frame
        void Update()
        {
            // --------------- Action --------------------------

            /*bool jumpEnded = Input.GetKeyDown("right shift"); //To change when merging !
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isSitting = animator.GetBool(isSittingHash);
        bool isSquatting = animator.GetBool(isSquattingHash);
        
        bool isWalki = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWkngBack = animator.GetBool(isWkngBackHash);
        bool isSideWalkingL = animator.GetBool(isSWLHash);
        bool isSideWalkingR = animator.GetBool(isSWRHash);
        bool isLF = animator.GetBool(isLFHash);
        bool isRF = animator.GetBool(isRFHash);*/


            /*// --------------- Key detection -------------------
        //One Press Needed                                   
        bool sitPress = Input.GetKeyDown("c"); 
        bool squatPress = Input.GetKeyDown("x");
        bool jumpPress = Input.GetKeyDown("space");
        
        //Pressing Needed
        bool walkPress = Input.GetKey("z");
        bool leftPress = Input.GetKey("q");
        bool rightPress = Input.GetKey("d");
        bool backPress = Input.GetKey("s");
        bool runPress = Input.GetKey("left shift");

        
        // -----------  State Action -----------
        if (squatPress)
            animator.SetBool(isSquattingHash,!isSquatting);

        if (sitPress)
            animator.SetBool(isSittingHash,!isSitting);
        
        //To change when merging !
        if (isJumping && jumpEnded)
            animator.SetBool(isJumpingHash,false);
        
        // ----------- Action -----------   
        // - Walking
        animator.SetBool(isWalkingHash, walkPress);
        
        // - Running
        animator.SetBool(isRunningHash, runPress && walkPress);
        animator.SetBool(isRunningHash, !(!runPress || !walkPress));

        // - W Back
        animator.SetBool(isWkngBackHash, backPress);

        // - Left
        animator.SetBool(isSWLHash, leftPress);

        // - Right
        animator.SetBool(isSWRHash, rightPress);

        // - Left & Forward (LF)
        animator.SetBool(isLFHash, walkPress && leftPress);
        animator.SetBool(isLFHash, !(!walkPress || !leftPress));

        // - Right & Forward (RF)
        animator.SetBool(isRFHash, walkPress && rightPress);
        animator.SetBool(isRFHash, !(!walkPress || !rightPress));

        // -----------  Jump  -----------
        if (!isSitting && jumpPress)
            animator.SetBool(isJumpingHash,true);*/
        
        }
    }
}
