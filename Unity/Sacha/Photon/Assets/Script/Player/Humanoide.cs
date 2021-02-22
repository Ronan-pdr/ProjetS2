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
    protected bool Grounded;
    
    public void SetGroundedState(bool grounded)
    {
        Grounded = grounded;
    }


    protected abstract void Awa();

    protected abstract void Sta();
    protected abstract void Upd();
    
    protected abstract void FixedUpd();
}
