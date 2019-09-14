using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleConroller : MonoBehaviour
{
    public LevelModuleStatus moduleStatus;

    public List<GameObject> saveTransformObjects;
    public List<GameObject> keyObjects;
    public List<UsingObject> usingObjects;

    private void Start()
    {
        Save();
    }

    public void Save()
    {
        moduleStatus.gameObjectActiveList = new List<bool>();
        foreach (var item in keyObjects)
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
    }

    public void Load()
    {
        for (int i = 0; i < moduleStatus.gameObjectActiveList.Count; i++)
        {
            keyObjects[i].SetActive(moduleStatus.gameObjectActiveList[i]);
        }

        for (int i = 0; i < moduleStatus.moduleStatusList.Count; i++)
        {
            if (!moduleStatus.moduleStatusList[i] && usingObjects[i].used)
            {
                usingObjects[i].ToStart();
            }
        }

        for (int i = 0; i < moduleStatus.savedTransforms.Count; i++)
        {
            saveTransformObjects[i].transform.position = moduleStatus.savedTransforms[i].position;
            saveTransformObjects[i].transform.rotation = moduleStatus.savedTransforms[i].rotation;
        }
    }
}
