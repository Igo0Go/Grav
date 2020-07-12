using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(AcidReactor))]
public class CubeStructureEditor : Editor
{
    private AcidReactor acid;

    private void OnEnable()
    {
        acid = (AcidReactor)target;
    }
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("U"))
        {
            acid.CreateUp();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(60));
        if (GUILayout.Button("L", GUILayout.MinHeight(60)))
        {
            acid.CreateLeft();
        }
        EditorGUILayout.BeginVertical(GUILayout.MinHeight(60));
        if (GUILayout.Button("F", GUILayout.MinHeight(29)))
        {
            acid.CreateForward();
        }
        if (GUILayout.Button("B", GUILayout.MinHeight(29)))
        {
            acid.CreateBackward();
        }
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("R", GUILayout.MinHeight(60)))
        {
            acid.CreateRight();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("D"))
        {
            acid.CreateDown();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
#endif
}
