using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputKit))]
public class InputKitInspector : Editor
{
    private InputKit kit;
    private Vector2 keysScrollPos;
    private Vector2 axisScrollPos;
    private bool drawKeys;
    private bool drawAxis;
    private bool standard;
    private void OnEnable()
    {
        kit = (InputKit)target;
        drawKeys = true;
        drawAxis = true;
        standard = false;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();
        standard = EditorGUILayout.Foldout(standard, "Обычный инспектор");
        GUILayout.EndVertical();
        if(standard)
        {
            base.OnInspectorGUI();
        }
        else
        {
            #region Кнопки
            GUILayout.BeginVertical();
            drawKeys = EditorGUILayout.Foldout(drawKeys, "Кнопки");

            if (drawKeys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Название");
                GUILayout.Label("Клавиша");
                GUILayout.EndHorizontal();

                keysScrollPos = EditorGUILayout.BeginScrollView(keysScrollPos, GUILayout.Height(100));
                for (int i = 0; i < kit.keys.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    kit.keys[i].Name = GUILayout.TextField(kit.keys[i].Name, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
                    kit.keys[i].key = (KeyCode)EditorGUILayout.EnumPopup(kit.keys[i].key, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                    {
                        kit.keys.Remove(kit.keys[i]);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("+", GUILayout.MaxWidth(50), GUILayout.MinWidth(50)))
                {
                    kit.keys.Add(new KeyCodeContainer());
                }
            }

            GUILayout.EndVertical();
            #endregion
            #region Оси
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            drawAxis = EditorGUILayout.Foldout(drawAxis, "Оси");
            if (drawAxis)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Название");
                GUILayout.Label("Клавиша +");
                GUILayout.Label("Клавиша -");
                GUILayout.EndHorizontal();

                axisScrollPos = EditorGUILayout.BeginScrollView(axisScrollPos, GUILayout.MaxHeight(120));
                for (int i = 0; i < kit.axis.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    kit.axis[i].name = GUILayout.TextField(kit.axis[i].name, GUILayout.MaxWidth(70), GUILayout.MinWidth(70));
                    kit.axis[i].positiveButtonKeyIndex = EditorGUILayout.Popup(kit.axis[i].positiveButtonKeyIndex, kit.GetKeyNames(),
                        GUILayout.MaxWidth(100), GUILayout.MinWidth(60));
                    SelectAxisButton(kit.axis[i], kit.axis[i].positiveButtonKeyIndex, 1);
                    kit.axis[i].negativeButtonKeyIndex = EditorGUILayout.Popup(kit.axis[i].negativeButtonKeyIndex, kit.GetKeyNames(),
                        GUILayout.MaxWidth(100), GUILayout.MinWidth(60));
                    SelectAxisButton(kit.axis[i], kit.axis[i].negativeButtonKeyIndex, -1);
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                    {
                        kit.axis.Remove(kit.axis[i]);
                        break;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label("Чувствительность = " + kit.axis[i].sensivity);
                    kit.axis[i].sensivity = GUILayout.HorizontalSlider(kit.axis[i].sensivity, 0, 1);
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("+", GUILayout.MaxWidth(50), GUILayout.MinWidth(50)))
                {
                    kit.axis.Add(new AxisContainer());
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            #if UNITY_EDITOR
            GUILayout.Space(80);
            if (GUILayout.Button("Сохранить", GUILayout.MinWidth(80)))
            {
                EditorUtility.SetDirty(kit);
            }
            GUILayout.Space(80);
#endif
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }
    }

    #region Служебные
    private void SelectAxisButton(AxisContainer container, int indexInKitList, int buttonType)
    {
        if(buttonType == 1)
        {
            container.positiveButton = kit.keys[indexInKitList];
        }
        else
        {
            container.negativeButton = kit.keys[indexInKitList];
        }
    }
    #endregion
}

