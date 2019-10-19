using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPoint : MonoBehaviour
{
    [Tooltip("дочерний объект Adjust")] public Transform adjustPoint;
    [Tooltip("это трансформ родителя")] public Transform endPoint;
    [Tooltip("дочерний объект Mirror")] public Transform adjustMirror;
    public Color color = Color.white;
    [Range(0,1)]public float scale = 1;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, Vector3.one * scale);
    }
}
