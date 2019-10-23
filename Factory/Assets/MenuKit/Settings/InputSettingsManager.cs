using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class InputSettingsManager : MonoBehaviour
{
    public InputKit inputKit;

    private void Update()
    {
        inputKit.InputUpdate();
    }

    public void CheckAxis()
    {
        for (int i = 0; i < inputKit.axis.Count; i++)
        {
            inputKit.axis[i].negativeButton.key = GetKey(inputKit.axis[i].negativeButton.Name);
            inputKit.axis[i].positiveButton.key = GetKey(inputKit.axis[i].positiveButton.Name);
        }
    }
    public float GetAxis(string Name)
    {
        foreach (var item in inputKit.axis)
        {
            if(item.name.Equals(Name))
            {
                return item.InputValue;
            }
        }
        Debug.LogWarning("Не удалось найти ось " + Name + ". Проверьте написание!");
        return 0;
    }
    public KeyCode GetKey(string Name)
    {
        foreach (var item in inputKit.keys)
        {
            if (item.Name.Equals(Name))
            {
                return item.key;
            }
        }
        Debug.LogWarning("Не удалось найти клавишу " + Name + ". Проверьте написание!");
        return KeyCode.None;
    }
    public void CopySettings(InputKitContainer right)
    {
        inputKit.sensivityMultiplicator = right.sensivityMultiplicator;

        inputKit.keys.Clear();
        for (int i = 0; i < right.keys.Count; i++)
        {
            inputKit.keys.Add(new KeyCodeContainer(right.keys[i]));
        }

        inputKit.axis.Clear();
        for (int i = 0; i < right.axis.Count; i++)
        {
            inputKit.axis.Add(new AxisContainer(right.axis[i]));
        }
    }
}
