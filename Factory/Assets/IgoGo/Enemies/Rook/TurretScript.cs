using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetTracker
{
    Transform Target { get; }
    
    void SetTarget(Transform target);
    void ClearTarget();
}

[RequireComponent(typeof(BoxCollider))]
public class TurretScript : MonoBehaviour, ITargetTracker
{
    [Header("Для зоны видимости нужен триггер слоя IgnoreRaycast и тэга EnemyView")]
    public Transform body;
    public GameObject shootPoint;
    public LayerMask ignoreMask;
    [Range(0.1f, 3)] public float reloadTime = 1;
    [Range(0.1f, 5)] public float rotSpeed = 1;

    private Quaternion startRotation;
    private Transform _target;
    private bool reload;
    private bool rotToAngle;
    private float shootTime = 0.2f;

    private bool CorrectAngleForShoot => Vector3.Angle(transform.forward, (Target.position + Target.up) - transform.position) <= 15;
    private bool CorrectAngleForDefault => Quaternion.Angle(transform.rotation, startRotation) <=1;
    private bool ISeeTarget => Physics.Raycast(transform.position, (Target.position + Target.up) - transform.position, ignoreMask);

    public Transform Target => _target;

    void Start()
    {
        startRotation = body.transform.rotation;
        reload = false;
        shootPoint.SetActive(false);
        rotToAngle = false;
    }

    void Update()
    {
        RotUpdate();
    }

    private void RotUpdate()
    {
        if(rotToAngle)
        {
            if (Target != null)
            {
                if (CorrectAngleForShoot)
                {
                    body.transform.LookAt(Target.position + Target.up);
                    if (!reload)
                    {
                        Shoot();
                    }
                }
                else
                {
                    RotToTarget();
                }

                if (Physics.Raycast(transform.position, (Target.position + Target.up) - transform.position, out RaycastHit hit, 100, ~ignoreMask))
                {
                    if(hit.transform == Target)
                    {
                        
                    }
                }
            }
            else
            {
                if (CorrectAngleForDefault)
                {
                    body.transform.rotation = startRotation;
                    rotToAngle = false;
                }
                else
                {
                    SmoothRot(startRotation);
                }
            }
        }
    }

    private void Shoot()
    {
        shootPoint.SetActive(true);
        reload = true;
        Invoke("StopShoot", shootTime);
        Invoke("Reload", reloadTime);
    }
    private void StopShoot() => shootPoint.SetActive(false);
    private void Reload() => reload = false;
    private void RotToTarget()
    {
        Quaternion targetRot = Quaternion.FromToRotation(body.transform.forward, (Target.position) - transform.position);
        SmoothRot(targetRot);
    }
    private void SmoothRot(Quaternion targetRot)
    {
        targetRot = body.transform.rotation * targetRot;
        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, targetRot, rotSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        rotToAngle = true;
    }
    public void ClearTarget() => _target = null;
}


