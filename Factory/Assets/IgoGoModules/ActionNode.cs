using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : UsingOrigin {

    [Tooltip("Входные элементы. У всех должен хотя раз быть вызван метод Use(), чтобы запустился этот модуль.")]
    public UsingOrigin[] actors;

    private int counter;

    private void Start()
    {
        counter = 0;
    }

    public override void Use()
    {
        counter++;
        if(counter == actors.Length)
        {
            UseAll();
        }
    }
    public void UseAll()
    {
        for (int i = 0; i < actionObjects.Length; i++)
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
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.3f);
            for (int i = 0; i < actionObjects.Length; i++)
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

