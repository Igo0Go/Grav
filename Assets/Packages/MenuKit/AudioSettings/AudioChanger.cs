using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioChanger : MonoBehaviour
{
    [SerializeField] private AudioType audioType = AudioType.Sound;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();

        AudioSettingsPanelScript audioSettingsPanel =
           FindObjectOfType<AudioSettingsPanelScript>().GetComponent<AudioSettingsPanelScript>();

        if (audioType == AudioType.Sound)
        {
            audioSettingsPanel.ChangeOtherVolume += VolumeUpdate;
        }
        else
        {
            audioSettingsPanel.ChangeMusicVolume += GetComponent<MusicManager>().AudioUpdate;
        }
    }

    //private void OnDestroy()
    //{
    //    AudioSettingsPanelScript audioSettingsPanel =
    //        FindObjectOfType<AudioSettingsPanelScript>().GetComponent<AudioSettingsPanelScript>();

    //    if (audioType == AudioType.Sound)
    //    {
    //        audioSettingsPanel.ChangeOtherVolume -= VolumeUpdate;
    //    }
    //    else
    //    {
    //        audioSettingsPanel.ChangeMusicVolume -= GetComponent<MusicManager>().AudioUpdate;
    //    }
    //}


    public void VolumeUpdate(float value)
    {
        if(source != null)
        {
            source.volume = value;
        }
    }
}

public enum AudioType
{
    Music,
    Sound
}
