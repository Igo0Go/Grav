using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetTracker
{
    Transform Target { get; }
    
    void SetTarget(Transform target);
    void ClearTarget(Transform target);
}

[RequireComponent(typeof(BoxCollider))]
public class TurretScript : MonoBehaviour, ITargetTracker
{
    #region Публичные поля
    [Header("Для зоны видимости нужен триггер слоя IgnoreRaycast и тэга EnemyView")]
    [Tooltip("Часть, которая будет непосредственно вращаться")] public Transform body;
    [Tooltip("Излучатель лазера")] public GameObject shootPoint;
    [Tooltip("Слои, на которые не будет реагировтаь сенсор турели")] public LayerMask ignoreMask;
    [Tooltip("Объект, который удет появляться во время отключения турели. К примеру, молнии")] public GameObject disactiveObj;
    [Tooltip("Объект, который является индикатором активности турели")] public GameObject chackActiveObject;
    [Range(0.1f, 3)] public float reloadTime = 1;
    [Range(0.1f, 5)] public float rotSpeed = 1;
    [Range(1, 30)] public float disactiveTime = 1;
    #endregion

    #region Служебные поля
    private Quaternion startRotation;
    private Transform _target;
    private bool reload;
    private bool rotToAngle;
    private bool disactive;
    private float shootTime = 0.2f;
    #endregion

    #region Свойства
    public Transform Target => _target;

    private bool CorrectAngleForShoot => Vector3.Angle(body.forward, (Target.position + Target.up) - transform.position) <= 10;
    private bool CorrectAngleForDefault => Quaternion.Angle(transform.rotation, startRotation) <=1;
    private bool ISeeTarget => Physics.Raycast(transform.position, (Target.position + Target.up) - transform.position, ignoreMask);
    #endregion

    #region Обработка событий Unity
    void Start()
    {
        disactive = false;
        startRotation = body.transform.rotation;
        reload = false;
        shootPoint.SetActive(false);
        rotToAngle = false;
        ReturnActive();
    }
    void Update()
    {
        RotUpdate();
    }
    #endregion

    #region Прицеливание и стрельба
    private void RotUpdate()
    {
        if(rotToAngle && !disactive)
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
        Quaternion targetRot = Quaternion.LookRotation(Target.position - transform.position);
        SmoothRot(targetRot);
    }
    private void SmoothRot(Quaternion targetRot)
    {
        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, targetRot, rotSpeed * Time.deltaTime);
    }
    #endregion

    #region Реакция на действия игрока
    public void Disactive()
    {
        if(!disactive)
        {
            disactiveObj.SetActive(true);
            chackActiveObject.SetActive(false);
            disactive = true;
            Invoke("ReturnActive", disactiveTime);
        }
    }
    public void SetTarget(Transform target)
    {
        _target = target;
        rotToAngle = true;
    }
    public void ClearTarget(Transform target) => _target = null;
    public void ReturnActive()
    {
        disactiveObj.SetActive(false);
        chackActiveObject.SetActive(true);
        disactive = false;
        disactiveObj.SetActive(false);
    }
    #endregion
}


