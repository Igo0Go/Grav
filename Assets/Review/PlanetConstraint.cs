using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetConstraint : MonoBehaviour
{
    public Transform planetTransform;

    private Rigidbody planetRb, rb;


    private void Start()
    {
        planetRb = planetTransform.GetComponent<Rigidbody>();
        rb = GetComponent < Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 directionToPlanet = (planetTransform.position - transform.position);

        float distance = directionToPlanet.magnitude;
        float strength = 10 * rb.mass * planetRb.mass / (distance * distance);
        rb.AddForce(directionToPlanet.normalized * strength);

        Quaternion rotBufer = Quaternion.FromToRotation(-transform.up, directionToPlanet.normalized);
        transform.rotation = rotBufer * transform.rotation;
    }
}
