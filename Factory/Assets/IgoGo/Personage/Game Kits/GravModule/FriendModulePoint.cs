﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendModulePoint : UsingOrigin
{

    [Tooltip("Позиция дрона при взломе")]public Transform friendPoint;
    [Range(0.01f, 60), Tooltip("Время взлома")]public float workTime = 1;

    

    public override void ToStart()
    {
        used = false;
    }

    public override void Use()
    {
        Invoke("UseAll", workTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (friendPoint != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(friendPoint.position, 0.4f);
        }
    }
    private void OnEnable()
    {
        tag = "DronModule";
    }

    private void UseAll()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
}
