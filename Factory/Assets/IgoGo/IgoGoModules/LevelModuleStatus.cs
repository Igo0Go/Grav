using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

[Serializable]
[XmlType("PosPack")]
public class PosPack
{
    [XmlElement("Position")]
    public Vector3 position;
    [XmlElement("Rotation")]
    public Vector3 rotation;
}

[XmlType("LevelModuleStatus")]
[XmlInclude(typeof(PosPack))]
public class LevelModuleStatus
{
    [XmlElement("Scene name")]
    public string sceneName;

    [XmlArray("SavedTransforms")]
    [XmlArrayItem("SavedTransformItem")]
    public List<PosPack> savedTransforms;

    [XmlArray("SavedActiveStatus")]
    [XmlArrayItem("SavedActiveStatusItem")]
    public List<bool> gameObjectActiveList;

    [XmlArray("SavedModuleStatus")]
    [XmlArrayItem("SavedModuleStatusItem")]
    public List<bool> moduleStatusList;

    [XmlArray("SavedLootStatus")]
    [XmlArrayItem("SavedLootStatusItem")]
    public List<bool> lootState;

    [XmlElement("Bank card status")]
    public byte bankCardStatus;

    public LevelModuleStatus() {}

    public LevelModuleStatus(string name)
    {
        sceneName = name;
        savedTransforms = new List<PosPack>();
        gameObjectActiveList = new List<bool>();
        moduleStatusList = new List<bool>();
        lootState = new List<bool>();
    }
}   

public static class LevelModuleStatusSettings
{
    public static List<LevelModuleStatus> levelModuleStatusList = new List<LevelModuleStatus>();

    public static LevelModuleStatus Find(string sceneName)
    {
        foreach (var item in levelModuleStatusList)
        {
            if(item.sceneName.Equals(sceneName))
            {
                return item;
            }
        }
        LevelModuleStatus result = new LevelModuleStatus(sceneName);
        levelModuleStatusList.Add(result);
        return result;
    }
}
