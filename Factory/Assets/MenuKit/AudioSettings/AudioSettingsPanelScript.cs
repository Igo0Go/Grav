using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AudioSettingsPanelScript : MonoBehaviour
{
    public InputSettingsManager manager;
    [Space(10)] public Slider musicSlider;
    public Slider otherAudioSlider;
    public GameObject audioSettingsPanel;

    [Space(20)] public List<AudioChanger> audioChangers;
    public List<MusicManager> musicManagers;

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
        if (Input.GetKeyDown(manager.GetKey("Cancel")) && audioSettingsPanel.activeSelf)
        {
            GetAudioPanel();
        }
    }


    public void Initialize()
    {
        musicSlider.value = MusicVolume;
        otherAudioSlider.value = OtherVolume;

        musicSlider.onValueChanged.AddListener(CallMusic);
        otherAudioSlider.onValueChanged.AddListener(CallOther);

        ChangeMusicVolume = ChangeOtherVolume = null;

        foreach (var item in musicManagers)
        {
            ChangeMusicVolume += item.AudioUpdate;
        }
        foreach (var item in audioChangers)
        {
            ChangeOtherVolume += item.VolumeUpdate;
        }
        ChangeOtherVolume?.Invoke(AudioSettingsPack.otherAudioMultiplicator);
        ChangeMusicVolume?.Invoke(AudioSettingsPack.musicMultiplicator);

        audioSettingsPanel.SetActive(mainMenuMode);
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

    private void CallMusic(float value) => MusicVolume = musicSlider.value;
    private void CallOther(float value) => OtherVolume = otherAudioSlider.value;
}
