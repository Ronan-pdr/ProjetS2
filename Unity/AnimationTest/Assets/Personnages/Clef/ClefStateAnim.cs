using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClefStateAnim : MonoBehaviour
{
    Animator animator;
    int isWalkingHash, isRunningHash, isSquattingHash, isWkngBackHash, isJumpingHash,
        isSWLHash, isSWRHash,isLFHash,isRFHash, HitHash;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isSquattingHash = Animator.StringToHash("isSquatting");
        isWkngBackHash = Animator.StringToHash("isWalkingBack");
        isJumpingHash = Animator.StringToHash("isJumping");
        isSWLHash = Animator.StringToHash("isSideWalkingL");
        isSWRHash = Animator.StringToHash("isSideWalkingR");
        isLFHash = Animator.StringToHash("isLF"); //Diago
        isRFHash = Animator.StringToHash("isRF"); //Diago
        HitHash = Animator.StringToHash("Hit");
    }

    // Update is called once per frame
    void Update()
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
        //One Press Needed                                   
        bool squatPress = Input.GetKeyDown("x");
        bool jumpPress = Input.GetKeyDown("space");
        
        //Pressing Needed
        bool walkPress = Input.GetKey("z");
        bool leftPress = Input.GetKey("q");
        bool rightPress = Input.GetKey("d");
        bool backPress = Input.GetKey("s");
        bool runPress = Input.GetKey("left shift");

        //Clicking Needed
        bool Attack = Input.GetMouseButtonDown(0);
        
        // -----------  State Action -----------
        if (squatPress)
            animator.SetBool(isSquattingHash,!isSquatting);
        
        if (jumpPress)
            animator.SetBool(isJumpingHash, true);

        //To change when merging !
        if (isJumping && jumpEnded)
            animator.SetBool(isJumpingHash, false);
        
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

        // - Shooting
        animator.SetBool(HitHash, Attack);

    }
}
