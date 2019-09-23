using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendScript : MonoBehaviour
{
    public Transform friendPoint;
    [Range(1,10)] public float speed = 5;
    public bool activeState;

    private FriendModulePoint modulePoint;
    private Transform target = null;
    private Rigidbody rb;
    private int moveToTarget;

    public bool NearWithTarget => Vector3.Distance(transform.position, target.position) <= 2;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        moveToTarget = 0;
    }

    void Update()
    {
        
    }

    
    public void SetTarget(FriendModulePoint point)
    {
        modulePoint = point;
        target = point.transform;
        if(activeState)
        {
            moveToTarget = 1;
        }
    }
    public void ToManipItem()
    {

    }
    public void FromManipItem()
    {

    }

    private void MoveToTarget()
    {
        if(moveToTarget != 0)
        {
            if (NearWithTarget)
            {
                if(moveToTarget == 1)
                {
                    transform.position = target.position;
                }
            }
            else
            { 
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
                transform.LookAt(target.position);
            }
        }
    }
    private void ReturnToPoint()
    {
        modulePoint.usingObject.Use();
        target = friendPoint;
        moveToTarget = -1;
    }
    private void ToDefaultState()
    {

    }
    private void DefaultState()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    private void UseModule()
    {
     
    }
}
