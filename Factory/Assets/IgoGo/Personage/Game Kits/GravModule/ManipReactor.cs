﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipReactor : UsingOrigin
{
    public ManipItem manip;
    public bool once;

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
        if(once)
        {
            Invoke("Inactive", Time.fixedDeltaTime);
        }
    }

    public override void ToStart()
    {
        used = false;
    }
    private void Inactive() => gameObject.SetActive(false);
}
