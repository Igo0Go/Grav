using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AudioSettingsPanelScript : MonoBehaviour
{
    [SerializeField] private InputSettingsManager manager;
    [SerializeField, Space(10)] private Slider musicSlider;
    [SerializeField] private Slider otherAudioSlider;
    [SerializeField] private GameObject audioSettingsPanel;

    public event Action ReturnEvent;
    public event Action<float> ChangeMusicVolume;
    public event Action<float> ChangeOtherVolume;

    public bool mainMenuMode = false;


    private float MusicVolume
    {
        get
        {
            return AudioSettingsPack.musicMultiplicator;
        }
        set
        {
            AudioSettingsPack.musicMultiplicator = value;
            ChangeMusicVolume?.Invoke(value);
        }
    }
    private float OtherVolume
    {
        get
        {
            return AudioSettingsPack.otherAudioMultiplicator;
        }
        set
        {
            AudioSettingsPack.otherAudioMultiplicator = value;
            ChangeOtherVolume?.Invoke(value);
        }
    }

    void Start()
    {
        Initialize();
    }
    void Update()
    {
        if (Input.GetKeyDown(manager.GetKey("Pause")) && audioSettingsPanel.activeSelf)
        {
            GetAudioPanel();
        }
    }


    public void Initialize()
    {
        musicSlider.value = MusicVolume;
        otherAudioSlider.value = OtherVolume;

        //musicSlider.onValueChanged.AddListener(CallMusic);
        //otherAudioSlider.onValueChanged.AddListener(CallOther);

        ChangeOtherVolume?.Invoke(AudioSettingsPack.otherAudioMultiplicator);
        ChangeMusicVolume?.Invoke(AudioSettingsPack.musicMultiplicator);

        audioSettingsPanel.SetActive(false);
    }
    public void GetAudioPanel()
    {
        if (!audioSettingsPanel.activeSelf)
        {
            audioSettingsPanel.SetActive(true);
        }
        else
        {
            audioSettingsPanel.SetActive(false);
            ReturnEvent?.Invoke();
        }
    }

    public void CallMusic() => MusicVolume = musicSlider.value;
    public void CallOther() => OtherVolume = otherAudioSlider.value;
}
