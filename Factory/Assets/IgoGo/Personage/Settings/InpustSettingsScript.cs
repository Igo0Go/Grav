using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InpustSettingsScript : MonoBehaviour
{
    [Header("Элементы")]
    public GameObject settingsPannel;
    public GameObject changePanel;
    public List<Text> keyValuesTexts;
    public InputSettingsManager manager;

    [Space(20)]
    public bool checkButtons;

    private KeyCodeContainer container;
    private int fieldNumber;
    private bool input;

    void Start()
    {
        changePanel.SetActive(false);
        settingsPannel.SetActive(false);
        MyTime.Start();
        MyTime.TimeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(manager.GetKey("Cancel")))
        {
            GetSettingsPanel();
        }
    }

    private void OnGUI()
    {
        if (input)
        {
            GetKeyForKeyContainer();
        }
    }

    private bool CheckHeaders()
    {
        if(manager.inputKit.keys.Count != keyValuesTexts.Count)
        {
            Debug.LogError("Количество текстов вывода не равно количеству заявленных кнопок");
            return false;
        }
        return true;
    }
    private void GetHeaders()
    {
        if(CheckHeaders())
        {
            for (int i = 0; i < manager.inputKit.keys.Count; i++)
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
            MyTime.Pause();
            settingsPannel.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            MyTime.Start();
            settingsPannel.SetActive(false);
            changePanel.SetActive(false);
            manager.CheckAxis();
            input = false;
            Cursor.lockState = CursorLockMode.Locked;
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
        if(checkButtons && CheckHeaders())
        {
            GetHeaders();
            checkButtons = false;
        }
    }

}
