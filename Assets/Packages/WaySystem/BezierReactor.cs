using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerGravMoveController))]
public class BezierReactor : MyTools
{
    [Range(1,30)]public float speed = 1;

    private PlayerGravMoveController moveController;
    private BezierCurve curve;

    private Vector3 targetPoint;
    private int pointNumber;

    public float Distance => Vector3.Distance(transform.position, targetPoint);

    private void Start()
    {
        moveController = GetComponent<PlayerGravMoveController>();
        moveController.OnGroundEvent += ClearCurve;
        moveController.PlayerStateController.playerInputController.JumpInputEvent += JumpOffTheRails;
    }
  
    private void JumpOffTheRails()
    {
        if (moveController.Status == PlayerState.speceUse && !moveController.OnGround())
        {
            StopMove();
            moveController.rb.AddForce(transform.up, ForceMode.Impulse);
        }
    }
    private void FixedUpdate()
    {
        MoveToTarget();
    }

    private void GetNearPoint()
    {
        moveController.rb.useGravity = false;
        moveController.rb.velocity = Vector3.zero;
        if (curve != null && curve.bezierPath.Length > 0)
        {
            pointNumber = 0;
            targetPoint = curve.bezierPath[pointNumber];
            float currentDistance = Distance;
            for (int i = 0; i < curve.bezierPath.Length; i++)
            {
                if (Vector3.Distance(transform.position, curve.bezierPath[i]) < currentDistance)
                {
                    currentDistance = Vector3.Distance(transform.position, curve.bezierPath[i]);
                    targetPoint = curve.bezierPath[i];
                    pointNumber = i;
                }

            }
            moveController.PlayerStateController.Status = PlayerState.speceUse;
        }
    }
    private void MoveToTarget()
    {
        if(moveController.Status == PlayerState.speceUse)
        {
            if(Distance > 0.7f)
            {
                transform.position = Vector3.Slerp(transform.position, targetPoint, speed * Time.fixedDeltaTime);
            }
            else
            {
                transform.position = targetPoint;
                CheckTargetPoint();
            }
        }
    }
    private void CheckTargetPoint()
    {
        if(pointNumber < curve.bezierPath.Length-1)
        {
            pointNumber++;
            targetPoint = curve.bezierPath[pointNumber];
        }
        else
        {
            StopMove();
        }
    }
    private void StopMove()
    {
        moveController.rb.useGravity = true;
        moveController.rb.AddForce(transform.forward * 5 + transform.up * 2, ForceMode.Impulse);
        moveController.PlayerStateController.Status = PlayerState.active;
        moveController.RotateToGrav();
    }
    private void ClearCurve()
    {
        curve = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!moveController.OnGround() && moveController.Status == PlayerState.active && other.CompareTag("WayStarter"))
        {
            BezierCurve bufer = null;
            if (curve != null)
            {
                bufer = curve;
            }
            if (MyGetComponent(other.gameObject, out curve) || MyGetComponent(other.transform.parent.gameObject, out curve))
            {
                if(bufer == curve)
                {
                    return;
                }
                GetNearPoint();
            }
        }
    }
}
