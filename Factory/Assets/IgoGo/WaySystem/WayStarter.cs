using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierCurve))]
public class WayStarter : MonoBehaviour
{

    private BezierCurve curve;
    void Start()
    {
        curve = GetComponent<BezierCurve>();
    }

    void Update()
    {
        
    }
}
