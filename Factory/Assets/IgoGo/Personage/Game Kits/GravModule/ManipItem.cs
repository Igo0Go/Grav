using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipItem : MyTools
{
    private Rigidbody rb;

    private void OnEnable()
    {
        tag = "Manip";
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Module"))
        {
            if(MyGetComponent(other.gameObject, out ManipReactor manip))
            {
                if(manip.manip == this)
                {
                    manip.Use();
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Module"))
        {
            if (MyGetComponent(collision.gameObject, out ManipReactor manip))
            {
                if(rb.velocity.magnitude*rb.mass > 2)
                {
                    manip.Use();
                }
            }
        }
    }
}
