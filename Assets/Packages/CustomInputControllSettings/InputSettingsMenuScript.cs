using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Данный класс контролирует панель меню для смены раскладки. При старте панель сама заполняется элементами в соответствии с теми кнопками,
/// которые указаны в пакете. При переназначении здесь идёт отслеживание того, какую кнопку пользователь нажал.
/// </summary>
public class InputSettingsMenuScript : MonoBehaviour
{
    [Header("Элементы")]
    public GameObject settingsPannel;
    public GameObject changePanel;
    public InputSettingsManager manager;
    public Slider sensivitySlider;
    public GameObject itemPrefab;
    public Transform scrollViewContent;

    private List<InputSettingsMenuItem> items; 
    private KeyCodeContainer container;
    private int fieldNumber;
    private bool input;

    public event Action ReturnEvent;

    public void Initialize()
    {
        ClearAllItemsFromList();
        CheckItemList();
        sensivitySlider.value = manager.inputKit.sensivityMultiplicator;
        sensivitySlider.onValueChanged.AddListener(ChangeSensivity);
        changePanel.SetActive(false);
        settingsPannel.SetActive(false);
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
    
    private void GetKeyForKeyContainer()
    {
        if (Event.current.type == EventType.KeyDown || Event.current.isKey)
        {
            container.key = Event.current.keyCode;
            items[fieldNumber].buttonText.text = container.key.ToString();
            changePanel.SetActive(false);
            input = false;
        }
        else if (Event.current.shift)
        {
            container.key = KeyCode.LeftShift;
            items[fieldNumber].buttonText.text = container.key.ToString();
            changePanel.SetActive(false);
            input = false;
        }
        else if (Event.current.isMouse)
        {
            int mouseButton = Event.current.button;
            switch (mouseButton)
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
            items[fieldNumber].buttonText.text = container.key.ToString();
            changePanel.SetActive(false);
            input = false;
        }
    }
    private void ChangeSensivity(float value) => manager.inputKit.sensivityMultiplicator = sensivitySlider.value;
    private void CheckItemList()
    {
        if (items == null)
            items = new List<InputSettingsMenuItem>();

        for (int i = 0; i < manager.inputKit.keys.Count; i++)
        {
            if (items.Count < i + 1)
            {
                items.Add(Instantiate(itemPrefab, scrollViewContent).GetComponent<InputSettingsMenuItem>());
            }
            else if (items[i] == null)
            {
                items[i] = Instantiate(itemPrefab, scrollViewContent).GetComponent<InputSettingsMenuItem>();
            }
            items[i].buttonNumber = i;
            items[i].OnChangeClick += GetContainer;
            items[i].headerText.text = manager.inputKit.keys[i].titleForMenu;
            items[i].buttonText.text = manager.inputKit.keys[i].key.ToString();
        }
    }
    private void ClearAllItemsFromList()
    {
        int count = scrollViewContent.childCount;
        while (count > 0)
        {
            DestroyImmediate(scrollViewContent.GetChild(0).gameObject);
            count--;
        }
        if (items != null)
            items.Clear();
    }

    private void OnGUI()
    {//реакция на ввод кнопки (смена)
        if (input)
        {
            GetKeyForKeyContainer();
        }
    }
}
