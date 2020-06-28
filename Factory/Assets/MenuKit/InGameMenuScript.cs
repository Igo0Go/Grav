using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InGameMenuScript : MyTools
{
    public InputSettingsMenuScript settingsScript;
    public AudioSettingsPanelScript audioSettings;
    public GameObject menuPanel;
    public GravFPS player;


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
        audioSettings.Initialize();
        settingsScript.Initialize();
        menuPanel.SetActive(false);
        MyCursor.OpportunityToChange = true;
        MyCursor.LockState = CursorLockMode.Locked;
        MyCursor.Visible = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(settingsScript.manager.GetKey("Pause"))/* && !inSettings*/)
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
        if(player.inHub)
        {
            player.gravFPSUI.StatusPack.currentScene = player.gravFPSUI.StatusPack.hubScene = "MainMenu";
            player.ReturnStats();
            player.sceneManager.LoadNextScene();
        }
        else
        {
            player.LoadHubScene();
        }
    }

    private void GetMenuPanel()
    {
        if (!menuPanel.activeSelf)
        {
            MyTime.Pause();
            menuPanel.SetActive(true);
            MyCursor.LockState = CursorLockMode.None;
            MyCursor.Visible = true;
            player.inMenu = true;
        }
        else
        {
            MyTime.Start();
            menuPanel.SetActive(false);
            settingsScript.GetSettingsPanel(false);
            MyCursor.LockState = CursorLockMode.Locked;
            MyCursor.Visible = false;
            player.inMenu = false;
        }
    }
    private void OnReturnEvent()
    {
        inSettings = false;
    }
}