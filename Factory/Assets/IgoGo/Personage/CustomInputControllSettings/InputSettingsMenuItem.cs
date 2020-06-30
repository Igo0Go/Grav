using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSettingsMenuItem : MonoBehaviour
{
    public Text headerText;
    public Text buttonText;
    [HideInInspector] public int buttonNumber;

    public event Action<int> OnChangeClick;

    public void OnChangeButtonClick()
    {
        OnChangeClick?.Invoke(buttonNumber);
    }
}
