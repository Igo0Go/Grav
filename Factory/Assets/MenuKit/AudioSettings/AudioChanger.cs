using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioChanger : MonoBehaviour
{
    public AudioSource source;

    public void VolumeUpdate(float value)
    {
        source.volume = value;
    }
}
