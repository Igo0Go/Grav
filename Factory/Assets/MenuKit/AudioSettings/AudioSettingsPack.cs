using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/AudioSettingsPack")]
public class AudioSettingsPack : ScriptableObject
{
    [Tooltip("множитель для громкости музыки"), Range(0,1)] public float musicMultiplicator;
    [Tooltip("множитель для громкости музыки"), Range(0, 1)] public float otherAudioMultiplicator;
}
