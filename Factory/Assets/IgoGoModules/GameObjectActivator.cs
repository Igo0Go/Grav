using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActivator : UsingObject
{
    [Tooltip("Объекты, которые будут переключены")] public List<GameObject> gameObjects;
    [Tooltip("Значение свойсва SetActive у всех объектов после ктивции")] public bool state;
    [Tooltip("Уничтожаться после активации")] public bool once;
    public override void Use()
    {
        UseAl(state);
        state = !state;
        if(once)
        {
            Destroy(gameObject);
        }
    }
    private void UseAl(bool value)
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i] != null)
            {
                gameObjects[i].SetActive(value);
            }
            else
            {
                Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if(debug)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, 0.3f);
            if(state)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i] != null)
                {
                    Gizmos.DrawLine(transform.position, gameObjects[i].transform.position);
                }
                else
                {
                    Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
                }
            }
        }
    }
}
