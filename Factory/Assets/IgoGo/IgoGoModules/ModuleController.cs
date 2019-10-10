using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModuleController : MonoBehaviour
{
    [Tooltip("Ссылка на ScriptableObject")] public LevelModuleStatus moduleStatus;
    [Tooltip("Объекты, у которых нужно сохранить положение")] public List<GameObject> saveTransformObjects;
    [Tooltip("Объекты, у которых нужно сохранить показатель активности на сцене")] public List<GameObject> activeSelfObjects;
    [Tooltip("Объекты, у которых нужно сохранить показатель срабатывания модуля")] public List<UsingObject> usingObjects;
    [Tooltip("Объекты, у которых нужно сохранить float-показатель animActivator")] public List<AnimActivator> floatAnimItems;
    public List<UsingObject> firstActionObjects;
    [SerializeField, Space(20), Tooltip("Стереть всё")] private bool clearSaveStates;

    private bool firstCycle;
    private string sceneName;

    public bool loadOnStart;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        firstCycle = true;
        if(loadOnStart)
        {
            Load();
        }
        else
        {
            Save();
        }
        firstCycle = false;
    }
    private void OnDrawGizmosSelected()
    {
        if(clearSaveStates)
        {
            DefaultValues();
            clearSaveStates = false;
        }
    }

    public void Save()
    {
        moduleStatus.gameObjectActiveList = new List<bool>();
        foreach (var item in activeSelfObjects)
        {
            moduleStatus.gameObjectActiveList.Add(item.activeSelf);
        }

        moduleStatus.savedTransforms = new List<PosPack>();
        foreach (var item in saveTransformObjects)
        {
            moduleStatus.savedTransforms.Add(new PosPack() { position = item.transform.position, rotation = item.transform.rotation});
        }

        moduleStatus.moduleStatusList = new List<bool>();
        foreach (var item in usingObjects)
        {
            moduleStatus.moduleStatusList.Add(item.used);
        }

        moduleStatus.animValues = new List<float>();
        foreach (var item in floatAnimItems)
        {
            moduleStatus.animValues.Add(item.target);
        }

        SaveSceneState();
    }
    public void Load()
    {
        if(firstCycle)
        {
            LoadSceneState();
        }

        for (int i = 0; i < moduleStatus.gameObjectActiveList.Count; i++)
        {
            activeSelfObjects[i].SetActive(moduleStatus.gameObjectActiveList[i]);
        }

        if(!firstCycle)
        {
            for (int i = 0; i < moduleStatus.moduleStatusList.Count; i++)
            {
                if (!moduleStatus.moduleStatusList[i] && usingObjects[i].used)
                {
                    usingObjects[i].ToStart();
                }
            }
        }

        for (int i = 0; i < moduleStatus.savedTransforms.Count; i++)
        {
            saveTransformObjects[i].transform.position = moduleStatus.savedTransforms[i].position;
            saveTransformObjects[i].transform.rotation = moduleStatus.savedTransforms[i].rotation;
        }

        for (int i = 0; i < moduleStatus.animValues.Count; i++)
        {
            floatAnimItems[i].SetActiveForAll(moduleStatus.animValues[i]);
        }
    }
    public void DefaultValues()
    {
        sceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < firstActionObjects.Count; i++)
        {
            PlayerPrefs.SetInt(sceneName + "UsOb" + i, 0);
        }
    }

    private void SaveSceneState()
    {
        for (int i = 0; i < firstActionObjects.Count; i++)
        {
            PlayerPrefs.SetInt(sceneName + "UsOb" + i, firstActionObjects[i].used ? 1 : 0);
        }
    }
    private void LoadSceneState()
    {
        for (int i = 0; i < firstActionObjects.Count; i++)
        {
            if(PlayerPrefs.GetInt(sceneName + "UsOb" + i) == 1)
            {
                firstActionObjects[i].Use();
            }
        }
    }
}
