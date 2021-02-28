using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotClass : Humanoide
{
    protected void AwakeIA()
    {
        Rb = GetComponent<Rigidbody>();
        Tr = GetComponent<Transform>();
    }

    protected override void SearchAnimation(string touche)
    {}
}
