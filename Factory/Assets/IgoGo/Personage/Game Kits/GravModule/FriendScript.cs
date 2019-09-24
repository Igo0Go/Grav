using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FriendScript : MonoBehaviour
{
    public GravityThrowerScript gravityThrower;
    public Transform friendSystemBufer;
    public Transform friendPoint;
    [Range(1,10)] public float speed = 5;
    public bool activeState;

    private FriendModulePoint modulePoint;
    private Transform target = null;
    private Rigidbody rb;
    private InputSettingsManager inputSettingsManager;
    private Transform player;
    private int moveToTarget;

    public bool NearWithTarget => Vector3.Distance(transform.position, target.position) <= 0.3f;


    void Start()
    {
        gravityThrower.ISeeDronPointEvent += SetTarget;
        gravityThrower.player.RotEvent += CheckRotate;
        inputSettingsManager = gravityThrower.player.inputSettingsManager;
        player = gravityThrower.player.transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        moveToTarget = 0;
    }

    void Update()
    {
        MoveToTarget();
        ChangeState();
        CheckSystemOffset();
    }

    public void CheckSystemOffset()
    {
        friendSystemBufer.transform.position = player.transform.position;
    }
    
    public void SetTarget(FriendModulePoint point)
    {
        if(activeState)
        {
            modulePoint = point;
            target = point.friendPoint;
            transform.parent = null;
            moveToTarget = 1;
        }
    }
    public void ToManipItem()
    {
        transform.parent = null;
    }
    public void ReturnToPoint()
    {
        target = friendPoint;
        moveToTarget = -1;
    }
    private void MoveToTarget()
    {
        if (moveToTarget != 0)
        {
            if (NearWithTarget)
            {
                transform.position = target.position;
                transform.rotation = target.rotation;
                if (moveToTarget == 1)
                {
                    modulePoint.Use();
                    moveToTarget = 2;
                    Invoke("ReturnToPoint", modulePoint.workTime);
                }
                else if (moveToTarget == -1)
                {
                    moveToTarget = 0;
                    transform.parent = target;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
                transform.LookAt(target.position);
            }
        }
    }
    
    private void ChangeState()
    {
        if(Input.GetKeyDown(inputSettingsManager.GetKey("ChangeUsing")) && moveToTarget == 0)
        {
            activeState = !activeState;
        }
    }
    private void CheckRotate(Quaternion rot)
    {
        friendSystemBufer.rotation = rot * friendSystemBufer.rotation;
    }
}
