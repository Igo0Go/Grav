using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlanetControl : MonoBehaviour
{
    public Transform cam;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 down = Vector3.Project(rb.velocity, transform.up);
        Vector3 forward = transform.forward * Input.GetAxis("Vertical") * 4;
        Vector3 right = transform.right * Input.GetAxis("Horizontal") * 4;

        rb.velocity = down + right + forward;

        transform.Rotate(transform.up, Input.GetAxis("Mouse X") * 2);
        cam.Rotate(-Input.GetAxis("Mouse Y") * 2, 0, 0);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * 10);
        }
    }
}
