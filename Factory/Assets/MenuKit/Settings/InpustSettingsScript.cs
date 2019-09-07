using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InpustSettingsScript : MonoBehaviour
{
    [Header("Элементы")]
    public GameObject settingsPannel;
    public GameObject changePanel;
    public InputSettingsManager manager;

    public List<Text> keyValuesTexts;

    [Space(20)]
    public bool checkButtons;

    private KeyCodeContainer container;
    private int fieldNumber;
    private bool input;

    public event Action ReturnEvent;

    void Start()
    {
        changePanel.SetActive(false);
        settingsPannel.SetActive(false);
        MyTime.Start();
        MyTime.TimeScale = 1;
        GetHeaders();
    }

    void Update()
    {
        if (Input.GetKeyDown(manager.GetKey("Cancel")) && settingsPannel.activeSelf)
        {
            GetSettingsPanel();
        }
    }

    private void OnGUI()
    {//реакция на ввод кнопки
        if (input)
        {
            GetKeyForKeyContainer();
        }
    }

    private bool CheckHeaders()
    {//сравнивает количество текстов, куда нужно будет вывести значения кнопок с количеством самих кнопок
        if(manager.inputKit.keys.Count < keyValuesTexts.Count)
        {
            Debug.LogError("Количество текстов вывода больше количества заявленных кнопок");
            return false;
        }
        return true;
    }
    private void GetHeaders()
    {//вписывает значения кнопок в соответствующие тексты по порядку с первой, если пройдена проверка
        if(CheckHeaders())
        {
            for (int i = 0; i < keyValuesTexts.Count; i++)
            {
                keyValuesTexts[i].text = manager.inputKit.keys[i].key.ToString();
            }
        }
    }
    private void GetKeyForKeyContainer()
    {
        if (Event.current.type == EventType.KeyDown || Event.current.isKey)
        {
            container.key = Event.current.keyCode;
            keyValuesTexts[fieldNumber].text = container.key.ToString();
            changePanel.SetActive(false);
            input = false;
        }
        else if (Event.current.shift)
        {
            container.key = KeyCode.LeftShift;
            keyValuesTexts[fieldNumber].text = container.key.ToString();
            changePanel.SetActive(false);
            input = false;
        }
        else if (Event.current.isMouse)
        {
            int mouseButton = Event.current.button;
            switch(mouseButton)
            {
                case 0:
                    container.key = KeyCode.Mouse0;
                    break;
                case 1:
                    container.key = KeyCode.Mouse1;
                    break;
                case 2:
                    container.key = KeyCode.Mouse2;
                    break;
                case 3:
                    container.key = KeyCode.Mouse3;
                    break;
                case 4:
                    container.key = KeyCode.Mouse4;
                    break;
                case 5:
                    container.key = KeyCode.Mouse5;
                    break;
                case 6:
                    container.key = KeyCode.Mouse6;
                    break;
            }
            keyValuesTexts[fieldNumber].text = container.key.ToString();
            changePanel.SetActive(false);
            input = false;
        }
    }
    public void GetSettingsPanel()
    {
        if (!settingsPannel.activeSelf)
        {
            settingsPannel.SetActive(true);
        }
        else
        {
            settingsPannel.SetActive(false);
            changePanel.SetActive(false);
            manager.CheckAxis();
            ReturnEvent?.Invoke();
            input = false;
        }
    }
    public void GetSettingsPanel(bool value)
    {
        settingsPannel.SetActive(value);
        if (!settingsPannel.activeSelf)
        {
            settingsPannel.SetActive(false);
            changePanel.SetActive(false);
            manager.CheckAxis();
            input = false;
        }
    }
    public void GetContainer(int number)
    {
        fieldNumber = number;
        container = manager.inputKit.keys[number];
        changePanel.SetActive(true);
        input = true;
    }

    private void OnDrawGizmosSelected()
    {
        if(checkButtons)
        {
            GetHeaders();
            checkButtons = false;
        }
    }

}
