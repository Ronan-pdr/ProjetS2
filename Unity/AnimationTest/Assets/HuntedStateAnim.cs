using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntedStateAnim : MonoBehaviour
{
    Animator animator;
    int isWalkingHash, isRunningHash, isSittingHash, isSquattingHash, isWkngBackHash, isJumpingHash,
        isSWLHash, isSWRHash,isLFHash,isRFHash;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isSittingHash = Animator.StringToHash("isSitting");
        isSquattingHash = Animator.StringToHash("isSquatting");
        isWkngBackHash = Animator.StringToHash("isWalkingBack");
        isJumpingHash = Animator.StringToHash("isJumping");
        isSWLHash = Animator.StringToHash("isSideWalkingL");
        isSWRHash = Animator.StringToHash("isSideWalkingR");
        isLFHash = Animator.StringToHash("isLF");
        isRFHash = Animator.StringToHash("isRF");
    }

    // Update is called once per frame
    void Update()
    {
        // --------------- Action --------------------------

        bool jumpEnded = Input.GetKeyDown("right shift"); //To change when merging !
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isSitting = animator.GetBool(isSittingHash);
        bool isSquatting = animator.GetBool(isSquattingHash);
        
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWkngBack = animator.GetBool(isWkngBackHash);
        bool isSideWalkingL = animator.GetBool(isSWLHash);
        bool isSideWalkingR = animator.GetBool(isSWRHash);
        bool isLF = animator.GetBool(isLFHash);
        bool isRF = animator.GetBool(isRFHash);


        // --------------- Key detection -------------------
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
            {
                if (isSquatting)
                    animator.SetBool(isSquattingHash,false);
                else
                    animator.SetBool(isSquattingHash,true);
            }

        if (sitPress)
        {
            if (isSitting)
                animator.SetBool(isSittingHash,false);
            else
                animator.SetBool(isSittingHash,true);
        }

        if (isJumping && jumpEnded)
            animator.SetBool(isJumpingHash ,false);
        
        // ----------- Action -----------   
        // - Walking
        if (!isWalking && walkPress)
            animator.SetBool(isWalkingHash, true);
        if (isWalking && !walkPress)
            animator.SetBool(isWalkingHash, false);   
        
        // - Running
        if (!isRunning && (runPress && walkPress))
            animator.SetBool(isRunningHash, true);
        if (isRunning && (!runPress || !walkPress))
            animator.SetBool(isRunningHash, false);

        // - W Back
        if (!isWkngBack && backPress)
            animator.SetBool(isWkngBackHash, true);
        if (isWkngBack && !backPress)
            animator.SetBool(isWkngBackHash, false);

        // - Left
        if (!isSideWalkingL && leftPress)
            animator.SetBool(isSWLHash, true);
        if (isSideWalkingL && !leftPress)
            animator.SetBool(isSWLHash, false);

        // - Right
        if (!isSideWalkingR && rightPress)
            animator.SetBool(isSWRHash, true);
        if (isSideWalkingR && !rightPress)
            animator.SetBool(isSWRHash, false);

        // - Left & Forward (LF)
        if (!isLF && (walkPress && leftPress))
            animator.SetBool(isLFHash, true);
        if (isLF && (!walkPress || !leftPress))
            animator.SetBool(isLFHash, false);

        // - Right & Forward (RF)
        if (!isRF && (walkPress && rightPress))
            animator.SetBool(isRFHash, true);
        if (isRF && (!walkPress || !rightPress))
            animator.SetBool(isRFHash, false);

        // -----------  Jump  -----------
        if (!isSitting && jumpPress)
            animator.SetBool(isJumpingHash,true);
        
    }
}
