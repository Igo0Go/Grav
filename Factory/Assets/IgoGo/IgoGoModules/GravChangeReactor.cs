using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravChangeReactor : UsingOrigin
{
    public GravFPS gravFPS;

    private void Start()
    {
        if(gravFPS == null)
        {
            Debug.LogError("Не был передан компонент gravFPS в " + name);
        }
        else
        {
            gravFPS.OnGravChange += Use;
        }
    }

    public override void ToStart()
    {
    }
    public override void Use()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
}
