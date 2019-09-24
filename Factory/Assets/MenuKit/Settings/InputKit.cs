using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            result[i] = keys[i].Name;
        }
        return result;
    }
}


[System.Serializable]
public class KeyCodeContainer
{
    public string Name;
    public KeyCode key;
}

[System.Serializable]
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
}




