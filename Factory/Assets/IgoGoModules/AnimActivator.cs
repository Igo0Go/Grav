using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimActivator : UsingObject {

    [Tooltip("Аниматор должен содержать параметр Active (bool)")]
    public Animator[] animObjects;
    
    [Tooltip("Начальное состояние")] public bool active;

    private void Start()
    {
        SetActiveForAll(active);
    }

    public override void Use()
    {
        active = !active;
        SetActiveForAll(active);
    }

    public void SetActiveForAll(bool value)
    {
        for (int i = 0; i < animObjects.Length; i++)
        {
            if(animObjects[i] != null)
            {
                animObjects[i].SetBool("Active", value);
            }
            else
            {
                Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.3f);
            for (int i = 0; i < animObjects.Length; i++)
            {
                if (animObjects[i] != null)
                {
                    Gizmos.DrawLine(transform.position, animObjects[i].transform.position);
                }
                else
                {
                    Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
                }
            }
        }
    }
}
