using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmeBlanche : Arme
{
    public override void Use()
    {
        Vector3 posController = controller.transform.position;
    }
}
