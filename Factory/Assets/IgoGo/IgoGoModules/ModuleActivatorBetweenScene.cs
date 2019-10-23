using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleActivatorBetweenScene : UsingObject
{
    public string sceneName;
    [Range(0,1000)]public int moduleIndexInScene;

    private LevelModuleStatus moduleStatus;

    public override void ToStart()
    {
        used = false;
        if(LevelModuleStatusSettings.Find(sceneName, out moduleStatus))
        {
            if(!(moduleStatus.moduleStatusList.Count > moduleIndexInScene))
            {
                Debug.LogError("На сцене " + sceneName + " не найден модуль с индексом " + moduleIndexInScene);
            }
        }
        else
        {
            Debug.LogError("Не найдена сцена " + sceneName);

        }
    }
    public override void Use()
    {
        moduleStatus.moduleStatusList[moduleIndexInScene] = true;
    }
    void Start()
    {
        ToStart();
    }
}
