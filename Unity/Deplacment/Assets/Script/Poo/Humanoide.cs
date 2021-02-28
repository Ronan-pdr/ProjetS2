using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Humanoide
{
    protected Transform Tr;
    protected Rigidbody Rb;
    public bool Ground = false;

    public abstract void Upd();
}
