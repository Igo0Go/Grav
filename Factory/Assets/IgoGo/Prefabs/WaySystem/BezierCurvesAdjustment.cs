using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]

public class BezierCurvesAdjustment : MonoBehaviour
{
    public Transform mirror;
    public Transform parent;
    public Color color = Color.white;
    public float scale = 1;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, Vector3.one * scale);
        Gizmos.DrawSphere(mirror.position, scale / 2);
        Gizmos.DrawLine(transform.position, mirror.position);
    }

    #if UNITY_EDITOR
    void LateUpdate()
    {
        mirror.position = parent.position + (transform.localPosition * -1);
    }
    #endif
}
