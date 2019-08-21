#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]

public class BezierCurveEditor : Editor
{
    BezierCurve curve;


    private void OnEnable()
    {
        curve = (BezierCurve)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(15);
        if (GUILayout.Button("Add New"))
        {
            curve.AddPoint();
            EditorUtility.SetDirty(curve);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Destroy Last"))
        {
            curve.DestroyLast();
            EditorUtility.SetDirty(curve);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Clear All"))
        {
            curve.ClearAll();
            EditorUtility.SetDirty(curve);
        }
    }
}
#endif
