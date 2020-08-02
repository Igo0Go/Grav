#if Unity_Editor
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(InputKit))]
public class InputKitInspector : Editor
{
    private InputKit kit;
    private Vector2 keysScrollPos;
    private Vector2 axisScrollPos;
    private bool drawKeys;
    private bool drawAxis;

    private void OnEnable()
    {
        kit = (InputKit)target;
        drawKeys = true;
        drawAxis = true;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.EndVertical();

        #region Кнопки
        GUILayout.BeginVertical();
        drawKeys = EditorGUILayout.Foldout(drawKeys, "Кнопки");

        if (drawKeys)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Название");
            GUILayout.Label("Клавиша");
            GUILayout.Label("Название в меню");
            GUILayout.EndHorizontal();

            keysScrollPos = EditorGUILayout.BeginScrollView(keysScrollPos, GUILayout.Height(100));
            for (int i = 0; i < kit.keys.Count; i++)
            {
                GUILayout.BeginHorizontal();
                kit.keys[i].name = GUILayout.TextField(kit.keys[i].name, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
                kit.keys[i].key = (KeyCode)EditorGUILayout.EnumPopup(kit.keys[i].key, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
                kit.keys[i].titleForMenu = GUILayout.TextField(kit.keys[i].titleForMenu, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
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
#if UNITY_EDITOR
                EditorUtility.SetDirty(kit);
#endif
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
        GUILayout.EndVertical();
        #endregion

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

#endif