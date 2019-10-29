using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleActivatorBetweenScene : UsingObject
{
    public string sceneName;
    public List<int> moduleIndexInSceneList;

    private LevelModuleStatus moduleStatus;

    public override void ToStart()
    {
        used = false;
        if(LevelModuleStatusSettings.Find(sceneName, out moduleStatus))
        {
            for (int i = 0; i < moduleIndexInSceneList.Count; i++)
            {
                if (!(moduleStatus.moduleStatusList.Count > moduleIndexInSceneList[i]))
                {
                    Debug.LogError("На сцене " + sceneName + " не найден модуль с индексом " + moduleIndexInSceneList[i]);
                }
            }
        }
        else
        {
            Debug.LogError("Не найдена сцена " + sceneName);
        }
    }
    public override void Use()
    {
        for (int i = 0; i < moduleIndexInSceneList.Count; i++)
        {
            moduleStatus.moduleStatusList[moduleIndexInSceneList[i]] = true;
        }
    }
    void Start()
    {
        ToStart();
    }
}
