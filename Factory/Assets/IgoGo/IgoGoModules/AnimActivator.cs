using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimActivator : UsingObject {

    [Tooltip("Аниматор должен содержать параметр Active (bool)")]
    public Animator[] animObjects;
    [SerializeField]private bool useFloat;
    [SerializeField, Range(0.01f, 1)] private float speed;


    [Tooltip("Начальное состояние")] public bool active;
    private bool currentActive;
    private float target;
    private float currentValue;
    private bool change;

    private void Start()
    {
        ToStart();
    }

    private void Update()
    {
        if(change)
        {
            currentValue += Time.deltaTime * speed;
            if(currentValue > target)
            {
                currentValue = target;
                change = false;
            }
            SetActiveForAll(currentValue);
        }
    }

    public override void Use()
    {
        if(useFloat)
        {
            target ++;
            change = true;
        }
        else
        {
            currentActive = !currentActive;
            SetActiveForAll(currentActive);
        }
        used = !used;
    }

    public override void ToStart()
    {
        used = false;
        if(useFloat)
        {
            currentValue = target = 0;
            SetActiveForAll(currentValue);
        }
        else
        {
            currentActive = active;
            SetActiveForAll(active);
        }
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
    public void SetActiveForAll(float value)
    {
        for (int i = 0; i < animObjects.Length; i++)
        {
            if (animObjects[i] != null)
            {
                animObjects[i].SetFloat("Value", value, Time.deltaTime, Time.deltaTime);
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
