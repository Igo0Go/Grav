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
            ChangeMusicVolume?.Invoke(audioSettings.musicMultiplicator);
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
            ChangeOtherVolume?.Invoke(audioSettings.otherAudioMultiplicator);
        }
    }

    void Start()
    {
        audioSettingsPanel.SetActive(false);
        musicSlider.value = MusicVolume;
        otherAudioSlider.value = OtherVolume;
        ChangeMusicVolume = ChangeOtherVolume = null;
        foreach (var item in musicManagers)
        {
            ChangeMusicVolume += item.AudioUpdate;
        }
        foreach (var item in audioChangers)
        {
            ChangeOtherVolume += item.VolumeUpdate;
        }
    }

    void Update()
    {
        if(audioSettingsPanel.activeSelf)
        {
            MusicVolume = musicSlider.value;
            OtherVolume = otherAudioSlider.value;
        }

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
}
