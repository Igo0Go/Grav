using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipReactor : UsingOrigin
{
    public ManipItem manip;

    private void OnEnable()
    {
        tag = "Module";
    }

    public override void Use()
    {
        for (int i = 0; i < actionObjects.Count; i++)
        {
            actionObjects[i].Use();
        }
        used = true;
    }

    public override void ToStart()
    {
        used = false;
    }
}
