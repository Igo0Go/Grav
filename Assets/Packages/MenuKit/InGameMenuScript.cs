using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InGameMenuScript : MyTools
{
    public InputSettingsMenuScript settingsScript;
    public AudioSettingsPanelScript audioSettings;
    public GameObject menuPanel;
    public PlayerStateController player;


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
        player.playerInputController.PauseInputEvent += OnPauseInput;
    }
    private void OnPauseInput()
    {
        if (!inSettings)
            GetMenuPanel();
    }

    public void Return()
    {
        if(!inSettings)
        {
            GetMenuPanel();
        }
    }
    public void GetInputSettings()
    {
        if(!inSettings)
        {
            inSettings = true;
            settingsScript.GetSettingsPanel();
        }
    }
    public void GetAudioSettings()
    {
        if(!inSettings)
        {
            inSettings = true;
            audioSettings.GetAudioPanel();
        }
    }
    public void Exit()
    {
        if(!inSettings)
        {
            if (player.InHub)
            {
                player.statusPack.currentScene = player.statusPack.hubScene = "MainMenu";
                player.playerSceneManagementController.ReturnStats();
                player.playerSceneManagementController.LoadNextScene();
            }
            else
            {
                player.playerSceneManagementController.LoadHubScene();
            }
        }
    }

    private void GetMenuPanel()
    {
        if (!menuPanel.activeSelf)
        {
            MyTime.Pause();
            menuPanel.SetActive(true);
            player.SetCursorVisible(true);
      
            player.InMenu = true;
        }
        else
        {
            MyTime.Start();
            menuPanel.SetActive(false);
            settingsScript.GetSettingsPanel(false);
            player.SetCursorVisible(false);
            player.InMenu = false;
        }
    }
    private void OnReturnEvent()
    {
        StartCoroutine(SetInSettingsKey(false));
    }

    private IEnumerator SetInSettingsKey(bool value)
    {
        yield return null;
        inSettings = value;
    }
}