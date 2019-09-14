using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BulletReactor : UsingOrigin
{

    [Tooltip("Уничтожаться после первого срабатывания")] public bool once;
    public override void Use()
    {
        UseAll();
        used = true;
        if (once)
        {
            Invoke("Disable", 0.3f);
        }
    }
    public override void ToStart()
    {
        used = false;
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

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if(debug)
        {
            Gizmos.color = Color.red;
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
