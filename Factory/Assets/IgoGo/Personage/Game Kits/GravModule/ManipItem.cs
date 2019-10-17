﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipItem : MyTools
{
    private Rigidbody rb;
    [HideInInspector] public bool damaged;

    private void OnEnable()
    {
        tag = "Manip";
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void ReturnDamaged() => damaged = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("ManipReactor"))
        {
            if(MyGetComponent(other.gameObject, out ManipReactor manip))
            {
                if(manip.manip == this)
                {
                    manip.Use();
                }
            }
        }
        else if (other.tag.Equals("EnemyView"))
        {
            other.GetComponent<ITargetTracker>().SetTarget(transform);
            return;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("EnemyView"))
        {
            other.GetComponent<ITargetTracker>().ClearTarget(transform);
            return;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Module"))
        {
            if (MyGetComponent(collision.gameObject, out ManipReactor manip))
            {
                if (manip.manip == null)
                {
                    if (damaged)
                    {
                        manip.Use();
                    }
                }
                else
                {
                    if(manip.manip == this && damaged)
                    {
                        manip.Use();
                    }
                }
            }
        }
        Invoke("ReturnDamaged", 0.1f);
    }
}
