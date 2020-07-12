using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModuleController : MonoBehaviour
{
    [Tooltip("Объекты, у которых нужно сохранить положение")] public List<GameObject> saveTransformObjects;
    [Tooltip("Объекты, у которых нужно сохранить показатель активности на сцене")] public List<GameObject> activeSelfObjects;
    [Tooltip("Объекты, у которых нужно сохранить показатель срабатывания модуля")] public List<UsingObject> usingObjects;
    [Tooltip("Все собираемые предметы будут удаляться, если уж были собраны при предыдущем посещении сцены")] public List<GameObject> lootObjects;
    [Tooltip("Статистики игрока")] public LootPointScript cardPoint;
    [SerializeField, Space(20), Tooltip("Стереть всё")] private bool clearSaveStates;

    [HideInInspector] public LevelModuleStatus moduleStatus;
    [HideInInspector] public List<UsingObject> firstActionObjects;

    [HideInInspector] public string sceneName;

    public bool loadOnStart;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        moduleStatus = LevelModuleStatusSettings.Find(sceneName);
        if(loadOnStart)
        {
            Load();
        }
        else
        {
            Save();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if(clearSaveStates)
        {
            sceneName = SceneManager.GetActiveScene().name;
            moduleStatus = LevelModuleStatusSettings.Find(sceneName);
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
            moduleStatus.savedTransforms.Add(new PosPack() { position = item.transform.position, rotation = item.transform.rotation.eulerAngles});
        }

        moduleStatus.moduleStatusList = new List<bool>();
        foreach (var item in usingObjects)
        {
            moduleStatus.moduleStatusList.Add(item.used);
        }

        moduleStatus.lootState = new List<bool>();
        foreach (var item in lootObjects)
        {
            moduleStatus.lootState.Add(item == null);
        }

        if(cardPoint != null)
        {
            moduleStatus.bankCardStatus = cardPoint.cardContains;
        }
    }
    public void Load()
    {
        for (int i = 0; i < moduleStatus.gameObjectActiveList.Count; i++)
        {
            activeSelfObjects[i].SetActive(moduleStatus.gameObjectActiveList[i]);
        }

        for (int i = 0; i < moduleStatus.moduleStatusList.Count; i++)
        {
            if (moduleStatus.moduleStatusList[i])
            {
                usingObjects[i].Use();
            }
            else
            {
                usingObjects[i].ToStart();
            }
        }

        for (int i = 0; i < moduleStatus.savedTransforms.Count; i++)
        {
            saveTransformObjects[i].transform.position = moduleStatus.savedTransforms[i].position;
            saveTransformObjects[i].transform.rotation = Quaternion.Euler(moduleStatus.savedTransforms[i].rotation);
        }

        for (int i = 0; i < moduleStatus.lootState.Count; i++)
        {
            if (moduleStatus.lootState[i])
            {
                Destroy(lootObjects[i]);
            }
        }
        if(cardPoint != null)
        {
            cardPoint.cardContains = moduleStatus.bankCardStatus;
        }
    }
    public void DefaultValues()
    {
        sceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < firstActionObjects.Count; i++)
        {
            PlayerPrefs.SetInt(sceneName + "UsOb" + i, 0);
        }

        moduleStatus.gameObjectActiveList.Clear();
        moduleStatus.moduleStatusList.Clear();
        moduleStatus.savedTransforms.Clear();
        moduleStatus.lootState.Clear();
    }
}
