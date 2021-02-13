using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public abstract class Humanoide : Entité
{
    public bool Ground = true;

    public abstract void Upd();
}
