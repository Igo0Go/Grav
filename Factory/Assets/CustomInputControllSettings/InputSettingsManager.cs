using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Данный класс является прослойкой между скриптами управления, создаваемыми вами и пакетом настроек управления.
/// Также здесь идёт обработка плавного изменения значений у осей.
/// </summary>
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
            inputKit.axis[i].negativeButton.key = GetKey(inputKit.axis[i].negativeButton.name);
            inputKit.axis[i].positiveButton.key = GetKey(inputKit.axis[i].positiveButton.name);
        }
    }
    /// <summary>
    /// Получить ось по её имени
    /// </summary>
    /// <param name="Name"></param>
    /// <returns>значения оси [-1; 1]</returns>
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
    /// <summary>
    /// Получить кнопку по имени
    /// </summary>
    /// <param name="Name"></param>
    /// <returns>получает конкретную клавишу в формате KeyCode</returns>
    public KeyCode GetKey(string Name)
    {
        foreach (var item in inputKit.keys)
        {
            if (item.name.Equals(Name))
            {
                return item.key;
            }
        }
        Debug.LogError("Не удалось найти клавишу " + Name + ". Проверьте написание!");
        return KeyCode.None;
    }
    /// <summary>
    /// Полностью перезаписывает текущий пакет и делает его копией переданного
    /// </summary>
    /// <param name="right"></param>
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
