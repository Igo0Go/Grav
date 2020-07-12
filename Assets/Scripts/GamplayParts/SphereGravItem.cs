using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SphereGravItem : MonoBehaviour
{
    [HideInInspector]public SphereGravModule sphere;

    public void DestroyMe()
    {
        DestroyImmediate(this);
    }
    public void InvokeOnBullet()
    {
        sphere.Use();
    }
}
