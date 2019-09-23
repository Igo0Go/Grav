using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DronScript : MonoBehaviour
{
    public GravityThrowerScript gravGun;
    public Transform dronPoint;

    private Transform currentTarget;
    private Rigidbody rb;
    private DronModule currentModule;
    private bool actionMode;
    private int moveStatus;

    private float Distance => Vector3.Distance(transform.position, currentTarget.position);

    private void OnEnable()
    {
        tag = "Dron";
    }
    void Start()
    {
        gravGun.secondUsing += ActionUse;
        actionMode = false;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }
    void Update()
    {
        MoveToTarget();
    }

    private void ActionUse(DronModule module)
    {
        currentModule = module;
        currentTarget = module.dronPoint;
        moveStatus = 1;
        transform.parent = null;
    }
    private void MoveToTarget()
    {
        if(moveStatus != 0)
        {
            if (Distance > 0.3f)
            {
                transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime);
            }
            else
            {
                transform.position = currentTarget.position;
                transform.rotation = currentTarget.rotation;

                if (moveStatus == 1)
                {
                    moveStatus = 2;
                    Invoke("ReturnToPlayer", currentModule.connectTime);
                }
                else if (moveStatus == -1)
                {
                    moveStatus = 0;
                    transform.parent = dronPoint;
                }
            }
        }
        rb.velocity = Vector3.zero;
    }
    private void ReturnToPlayer()
    {
        currentTarget = dronPoint;
        moveStatus = -1;
    }
}
