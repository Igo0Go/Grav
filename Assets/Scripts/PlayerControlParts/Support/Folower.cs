using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folower : MonoBehaviour
{
    public Transform target;

    private Transform myTransform;

    void Start()
    {
        myTransform = transform;
        myTransform.parent = null;
    }

    void LateUpdate()
    {
        myTransform.position = target.position;
    }
}
