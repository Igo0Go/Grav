using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ReturnTransformModule : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Water") || other.tag.Equals("Water"))
        {
            rb.velocity = Vector3.zero;
            transform.position = startPos;
        }
    }

}
