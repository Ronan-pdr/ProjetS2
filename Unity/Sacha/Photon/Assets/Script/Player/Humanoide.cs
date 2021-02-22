using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Humanoide : MonoBehaviour
{
    protected Transform Tr;
    protected Rigidbody Rb;
    public bool Grounded = false;
    
    public void SetGroundedState(bool grounded)
    {
        Grounded = grounded;
    }

    protected abstract void Upd();
    
    protected abstract void FixedUpd();
}
