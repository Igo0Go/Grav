using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : UsingObject
{
    public Transform objectTransform;
    public Transform toPoint;

    public override void ToStart()
    {

    }

    public override void Use()
    {
        objectTransform.position = toPoint.position;
        objectTransform.rotation = toPoint.rotation;
    }
}
