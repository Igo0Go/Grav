using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActionModule : UsingOrigin
{
    [Tooltip("Одноразовый")] public bool once;
    [Tooltip("Источники для модуля")] public List<UsingOrigin> actors;

    public override void ToStart()
    {
        used = false;
    }
    public override void Use()
    {
        if(actionObjects != null)
        {
            int index = Random.Range(0, actionObjects.Count);
            actionObjects[index].Use();
            used = true;
        }
        if(once)
        {
            foreach (var item in actors)
            {
                item.actionObjects.Remove(this);
            }
            Destroy(this);
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.3f);
            for (int i = 0; i < actionObjects.Count; i++)
            {
                if (actionObjects[i] != null)
                {
                    Gizmos.DrawLine(transform.position, actionObjects[i].transform.position);
                    Gizmos.DrawSphere(Vector3.Lerp(transform.position, actionObjects[i].transform.position,
                        Vector3.Distance(actionObjects[i].transform.position, transform.position) / 2), 0.3f);
                }
                else
                {
                    Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
                }
            }
        }
    }
}
