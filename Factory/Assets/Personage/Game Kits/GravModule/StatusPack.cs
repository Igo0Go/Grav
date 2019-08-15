using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/StatusPack")]
public class StatusPack : ScriptableObject
{
    public string currentScene;
    public string hubScene;

    public int loadStatus;

    public int money;
    public int lifeSphereCount;
}
