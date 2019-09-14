using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : UsingOrigin
{
    [Space(20), Tooltip("Время, через которое будет вызван метод Use()"), Range(0,float.MaxValue)]
    public float time;
    [SerializeField] private bool active;

    private float currentTime;

    private void Start()
    {
        if(active)
        {
            Use();
        }
    }

    private void Update()
    {
        if(used && active)
        {
            if(currentTime < time)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                UseAll();
                used = false;
                currentTime = 0;
            }
        }
    }

    public override void Use()
    {
        active = true;
        used = true;
        currentTime = 0;
    }
    public override void ToStart()
    {
        used = false;
        currentTime = 0;
    }

    public void UseAll()
    {
        for (int i = 0; i < actionObjects.Count; i++)
        {
            if (actionObjects[i] != null)
            {
                actionObjects[i].Use();
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
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(transform.position, 0.3f);
            for (int i = 0; i < actionObjects.Count; i++)
            {
                if (actionObjects[i] != null)
                {
                    Gizmos.DrawLine(transform.position, actionObjects[i].transform.position);
                }
                else
                {
                    Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
                }
            }
        }
    }
}
