using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGravModule : UsingObject
{
    public bool planetGravityType;
    public List<Rigidbody> rigidbodies;
    public GravFPS player;
    public float radius;

    private Rigidbody rb;
    private Vector3 gravVector;
    private int gravMultiplicator;


    private void Start()
    {
        gravMultiplicator = planetGravityType ? 1 : -1;
        rb = GetComponent<Rigidbody>();
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            radius = GetComponent<SphereCollider>().radius;
        }
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

    public override void Use()
    {
        if(player != null)
        {
            player.SetGravObj(this);
        }
    }

    public override void ToStart()
    {

    }
}
