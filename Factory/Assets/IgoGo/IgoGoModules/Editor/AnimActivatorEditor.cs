using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimActivator))]
public class AnimActivatorEditor : Editor
{
    private AnimActivator animActivator;
    private bool drawAnimObjects;

    private void OnEnable()
    {
        animActivator = (AnimActivator)target;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(animActivator, "Изменение animActivator " + target.name);

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Debug", GUILayout.MaxWidth(40));
        animActivator.debug = EditorGUILayout.Toggle(animActivator.debug);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Показывать анимируемые объекты", GUILayout.MaxWidth(220));
        drawAnimObjects = EditorGUILayout.Toggle(drawAnimObjects);
        GUILayout.EndHorizontal();
        if (drawAnimObjects)
        {
            if(animActivator.animObjects != null)
            {
                for (int i = 0; i < animActivator.animObjects.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    animActivator.animObjects[i] = (Animator)EditorGUILayout.ObjectField(animActivator.animObjects[i], typeof(Animator), true);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("x", GUILayout.MaxHeight(30)))
                    {
                        animActivator.animObjects.Remove(animActivator.animObjects[i]);
                        i--;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("+", GUILayout.MaxHeight(30), GUILayout.MaxWidth(120), GUILayout.MinWidth(40)))
            {
                if (animActivator.animObjects == null)
                {
                    animActivator.animObjects = new List<Animator>();
                }
                animActivator.animObjects.Add(null);
            }
            EditorGUILayout.Space();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Использовать float в анимации", GUILayout.MaxWidth(190));
        animActivator.useFloat = EditorGUILayout.Toggle(animActivator.useFloat);
        GUILayout.EndHorizontal();
        if (animActivator.useFloat)
        {
            animActivator.speed = EditorGUILayout.Slider(animActivator.speed, 0.1f, 1);
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Статус активности", GUILayout.MaxWidth(120));
        animActivator.active = EditorGUILayout.Toggle(animActivator.active);
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
    }
}
