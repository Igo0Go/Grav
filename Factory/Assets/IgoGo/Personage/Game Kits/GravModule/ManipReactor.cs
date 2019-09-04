using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipReactor : UsingOrigin
{
    public ManipItem manip;

    public override void Use()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
            used = true;
        }
    }

    public override void ToStart()
    {
        used = false;
    }
}
