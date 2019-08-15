using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : UsingObject
{
    [Space(20)]
    [Tooltip("Какой номер из указанных в MusicManager будет играть после активации")] public int musicNumber;
    [Tooltip("Уничтожаться после использования")] public bool destroyed;
    [Tooltip("Ссылка на MusicManager")] public MusicManager manager;

    public override void Use()
    {
        if(musicNumber < manager.musicBoxes.Length)
        {
            manager.CurrentBox = musicNumber;
            if (destroyed)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("Для MusicManager " + manager.name + " невозможно запустить трэк под номером " + musicNumber + ". Источник ошибки:" +
                gameObject.name);
        }
    }
}
