using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : UsingObject
{
    [Space(20)]
    [Tooltip("Какой номер из указанных в MusicManager будет играть после активации")] public int musicNumber;
    [Tooltip("Ссылка на MusicManager")] public MusicManager manager;

    public override void Use()
    {
        if(musicNumber < manager.musicBoxes.Length)
        {
            used = true;
            manager.CurrentBox = musicNumber;
        }
        else
        {
            Debug.LogError("Для MusicManager " + manager.name + " невозможно запустить трэк под номером " + musicNumber + ". Источник ошибки:" +
                gameObject.name);
        }
    }
    public override void ToStart()
    {
        used = false;
    }

}
