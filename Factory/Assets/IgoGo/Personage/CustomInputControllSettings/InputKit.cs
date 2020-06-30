using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System;



/// <summary>
/// Данный класс используется для хранения настроек управления. ScriptableObject позволяет создавать паки для разных форм управления.
/// К примеру, может быть раскладка для шутера от первого лица с прыжками и пакет для управления техникой в этом же проекте.
/// </summary>
[XmlType("InputKit")]
[XmlInclude(typeof(KeyCodeContainer))]
[XmlInclude(typeof(AxisContainer))]
[CreateAssetMenu(menuName = "Config/InputKit")]
public class InputKit : ScriptableObject
{
    public float sensivityMultiplicator;
    public List<KeyCodeContainer> keys;
    public List<AxisContainer> axis;

    public void InputUpdate()
    {
        foreach (var item in axis)
        {
            item.GetInputValue();
        }
    }

    public string[] GetKeyNames()
    {
        string[] result = new string[keys.Count];
        for (int i = 0; i < keys.Count; i++)
        {
            result[i] = keys[i].name;
        }
        return result;
    }
}

/// <summary>
/// Класс для хранения кнопки
/// </summary>
[XmlType("KeyCodeContainer")]
[Serializable]
public class KeyCodeContainer
{
    public string name;
    public string titleForMenu;
    public KeyCode key;

    public KeyCodeContainer() { }
    public KeyCodeContainer(KeyCodeContainer blueprint)
    {
        name = blueprint.name;
        key = blueprint.key;
    }
}

/// <summary>
/// класс для хранения оси, состоящей из двух кнопок (положительное и отрицательное значение оси)
/// </summary>
[XmlType("AxisContainer")]
[XmlInclude(typeof(KeyCodeContainer))]
[Serializable]
public class AxisContainer
{
    public int positiveButtonKeyIndex;
    public int negativeButtonKeyIndex;
    public string name;
    public KeyCodeContainer positiveButton; //кнопка для 1
    public KeyCodeContainer negativeButton; //кнопка для -1

    [Range(0, 1)]
    public float sensivity; //скорость нажатия

    public float InputValue
    {
        get { return _inputValue; }
    } //текущее значение нажатия
    private float _inputValue;
    public void GetInputValue()
    {
        if (Input.GetKey(positiveButton.key) || Input.GetKey(negativeButton.key))
        {
            if (Input.GetKey(positiveButton.key))
            {
                _inputValue = Mathf.Clamp(_inputValue + sensivity, -1, 1);
            }
            if (Input.GetKey(negativeButton.key))
            {
                _inputValue = Mathf.Clamp(_inputValue - sensivity, -1, 1);
            }
        }
        else
        {
            _inputValue = Mathf.Lerp(_inputValue, 0, sensivity);
        }
    }
    public void SetSensivity(float value)
    {
        sensivity = value;
    }

    public AxisContainer() { }
    public AxisContainer(AxisContainer blueprint)
    {
        positiveButtonKeyIndex = blueprint.positiveButtonKeyIndex;
        negativeButtonKeyIndex = blueprint.negativeButtonKeyIndex;
        name = blueprint.name;
        positiveButton = new KeyCodeContainer(blueprint.positiveButton);
        negativeButton = new KeyCodeContainer(blueprint.negativeButton);
        SetSensivity(blueprint.sensivity);
    }
}