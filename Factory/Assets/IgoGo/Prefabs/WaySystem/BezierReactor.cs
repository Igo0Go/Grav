using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravFPS))]
public class BezierReactor : MyTools
{
    [Range(1,30)]public float speed = 1;

    private GravFPS gravFPS;
    private BezierCurve curve;

    private Vector3 targetPoint;
    private int pointNumber;

    public float Distance => Vector3.Distance(transform.position, targetPoint);

    private void Start()
    {
        gravFPS = GetComponent<GravFPS>();
        gravFPS.OnGroundEvent += ClearCurve;
    }
    private void Update()
    {
        if (gravFPS.Status == PlayerState.speceUse && !gravFPS.OnGround())
        {
            if (Input.GetKeyDown(gravFPS.inputSettingsManager.GetKey("Jump")))
            {
                StopMove();
                gravFPS.rb.AddForce(transform.up, ForceMode.Impulse);
            }
        }
    }
    private void FixedUpdate()
    {
        MoveToTarget();
    }

    private void GetNearPoint()
    {
        gravFPS.rb.useGravity = false;
        gravFPS.rb.velocity = Vector3.zero;
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
            gravFPS.Status = PlayerState.speceUse;
        }
    }
    private void MoveToTarget()
    {
        if(gravFPS.Status == PlayerState.speceUse)
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
        gravFPS.rb.useGravity = true;
        gravFPS.rb.AddForce(transform.forward * 5 + transform.up * 2, ForceMode.Impulse);
        gravFPS.Status = PlayerState.active;
        gravFPS.RotateToGrav();
    }
    private void ClearCurve()
    {
        curve = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!gravFPS.OnGround() && gravFPS.Status == PlayerState.active && other.tag.Equals("WayStarter"))
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
