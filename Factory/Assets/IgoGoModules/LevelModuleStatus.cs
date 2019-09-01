using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PosPack
{
    public Vector3 position;
    public Quaternion rotation;
}

[CreateAssetMenu(menuName = "Config/LevelModuleStatus")]
public class LevelModuleStatus : ScriptableObject
{
    public List<PosPack> savedTransforms = new List<PosPack>();
    public List<bool> gameObjectActiveList = new List<bool>();
    public List<bool> moduleStatusList = new List<bool>();
}
