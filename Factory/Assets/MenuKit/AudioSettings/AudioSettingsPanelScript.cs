using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AudioSettingsPanelScript : MonoBehaviour
{
    public InputSettingsManager manager;
    public AudioSettingsPack audioSettings;
    [Space(10)] public Slider musicSlider;
    public Slider otherAudioSlider;
    public GameObject audioSettingsPanel;

    [Space(20)] public List<AudioChanger> audioChangers;
    public List<MusicManager> musicManagers;

    public event Action ReturnEvent;
    public event Action<float> ChangeMusicVolume;
    public event Action<float> ChangeOtherVolume;


    private float MusicVolume
    {
        get
        {
            return audioSettings.musicMultiplicator;
        }
        set
        {
            audioSettings.musicMultiplicator = value;
            ChangeMusicVolume?.Invoke(value);
        }
    }
    private float OtherVolume
    {
        get
        {
            return audioSettings.otherAudioMultiplicator;
        }
        set
        {
            audioSettings.otherAudioMultiplicator = value;
            ChangeOtherVolume?.Invoke(value);
        }
    }

    void Start()
    {
        audioSettingsPanel.SetActive(false);

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
        ChangeOtherVolume?.Invoke(audioSettings.otherAudioMultiplicator);
        ChangeMusicVolume?.Invoke(audioSettings.musicMultiplicator);
    }
    void Update()
    {
        if (Input.GetKeyDown(manager.GetKey("Cancel")) && audioSettingsPanel.activeSelf)
        {
            GetAudioPanel();
        }
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
