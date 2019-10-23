using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioChanger))]
public class AudioStarter : UsingObject
{
    private AudioSource source;

    public override void ToStart()
    {
        
    }

    public override void Use()
    {
        source.Play();
    }

    void Start()
    {
        source = GetComponent<AudioSource>();
    }
}
