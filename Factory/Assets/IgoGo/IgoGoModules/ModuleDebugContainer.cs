using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDebugContainer : MyTools
{
    [Tooltip("Значение Debug вложенных элементов")] public bool debug;
    [Tooltip("Элементы, на которые распространяется переключатель")] 
    public List<UsingObject> usingObjects;
    public List<ModuleDebugContainer> containers;

    [Space(20)]
    [Tooltip("Значение Debug вложенных элементов")] public bool addChildren;
    [Tooltip("Передать значение Debug")] public bool check;

    private void OnDrawGizmosSelected()
    {
        if(addChildren)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                UsingObject usingObject;
                if(MyGetComponent(transform.GetChild(i).gameObject, out usingObject))
                {
                    if(!usingObjects.Contains(usingObject))
                    {
                        usingObjects.Add(usingObject);
                    }
                }
                ModuleDebugContainer container;
                if (MyGetComponent(transform.GetChild(i).gameObject, out container))
                {
                    if (!containers.Contains(container))
                    {
                        containers.Add(container);
                    }
                }
            }
            for (int i = 0; i < containers.Count; i++)
            {
                if (containers[i] != null)
                {
                    containers[i].debug = debug;
                    containers[i].check = true;
                }
                else
                {
                    Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
                }
            }
            addChildren = false;
        }
        if (check)
        {
            for (int i = 0; i < usingObjects.Count; i++)
            {
                if (usingObjects[i] != null)
                {
                    usingObjects[i].debug = debug;
                }
                else
                {
                    Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
                }
            }
            for (int i = 0; i < containers.Count; i++)
            {
                if (containers[i] != null)
                {
                    containers[i].debug = debug;
                    containers[i].check = true;
                }
                else
                {
                    Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
                }
            }
            check = false;
        }
    }
}
