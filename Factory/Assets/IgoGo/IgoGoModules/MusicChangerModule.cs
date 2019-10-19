using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangerModule : UsingObject
{
    public int number;
    public MusicManager musicManager;

    public override void ToStart()
    {
    }

    public override void Use()
    {
        if(number < musicManager.musicBoxes.Length)
        {
            musicManager.CurrentBox = number;
        }
    }
}
