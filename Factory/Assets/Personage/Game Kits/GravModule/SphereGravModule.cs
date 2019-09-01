using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGravModule : MonoBehaviour
{
    public bool planetGravityType;
    public List<Rigidbody> rigidbodies;

    private Rigidbody rb;
    private Vector3 gravVector;
    private int gravMultiplicator;


    private void Start()
    {
        gravMultiplicator = planetGravityType ? 1 : -1;
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        foreach (var item in rigidbodies)
        {
            gravVector = gravMultiplicator * (transform.position - item.position);

            float distance = gravVector.magnitude;
            float strength = 10 * item.mass * rb.mass / (distance * distance);
            item.AddForce(gravVector.normalized * strength);
        }
    }
}
