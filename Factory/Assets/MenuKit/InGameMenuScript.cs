using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InGameMenuScript : MyTools
{
    public InpustSettingsScript settingsScript;
    public AudioSettingsPanelScript audioSettings;
    public GameObject menuPanel;

    private static bool inSettings;

    void Start()
    {
        inSettings = false;
        MyTime.TimeScale = 1;
        MyTime.Start();
        settingsScript.gameObject.SetActive(true);
        audioSettings.gameObject.SetActive(true);
        settingsScript.ReturnEvent += OnReturnEvent;
        audioSettings.ReturnEvent += OnReturnEvent;
        menuPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(settingsScript.manager.GetKey("Cancel"))/* && !inSettings*/)
        {
            if(!inSettings)
            GetMenuPanel();
        }
    }

    public void Return()
    {
        GetMenuPanel();
    }
    public void GetInputSettings()
    {
        inSettings = true;
        settingsScript.GetSettingsPanel();
    }
    public void GetAudioSettings()
    {
        inSettings = true;
        audioSettings.GetAudioPanel();
    }
    public void Exit()
    {
        Debug.Log("Вышли в окно");
    }

    private void GetMenuPanel()
    {
        if (!menuPanel.activeSelf)
        {
            MyTime.Pause();
            menuPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            MyTime.Start();
            menuPanel.SetActive(false);
            settingsScript.GetSettingsPanel(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void OnReturnEvent()
    {
        inSettings = false;
    }
}