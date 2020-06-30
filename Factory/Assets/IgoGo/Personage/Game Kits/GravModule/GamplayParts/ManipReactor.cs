using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipReactor : UsingOrigin
{
    [Tooltip("Точка вилки")] public Transform plugPoint;
    [Tooltip("Является розеткой")] public bool isSocket;
    [Tooltip("На какой объект реагировать")] public ManipItem manip;
    public bool once;

    public override void Use()
    {
        if(!used)
        {
            for (int i = 0; i < actionObjects.Count; i++)
            {
                actionObjects[i].Use();
            }
            used = true;
            if (once)
            {
                Invoke("Inactive", Time.fixedDeltaTime);
            }
        }
    }
    public override void ToStart()
    {
        used = false;
    }
    
    private void Inactive() => gameObject.SetActive(false);
}
