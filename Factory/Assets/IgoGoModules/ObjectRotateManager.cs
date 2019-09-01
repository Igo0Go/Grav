using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum RotateType
{
    AroundAxis,
    Reverse,
    Once
}

public class ObjectRotateManager : UsingObject
{
    [Space(20)]
    [Tooltip("Смещение от стартового вращения")] public Vector3 rotVector;
    [Tooltip("Скорость движения")] public float speed;
    [Tooltip("Задержка перед запуском")] public float delay;
    [Tooltip("Тип поворота")] public RotateType type;
    [Tooltip("Задержка между циклами")] public float pauseTime;
    [Tooltip("Активно сразу")] [Space(20)] public bool active;

    [Space(20)]
    [Header("Настройки дебага")]
    [Tooltip("Нужен для дебага. Создайте пустышку дочерним объектом.")] public Transform helper;
    [Tooltip("Дальность линии от центра"), Range(1,10)] public float range = 1;

    #region Служебные
    private Action rotHandler;
    private Quaternion startRot;
    private Quaternion endrot;
    private Quaternion currentTargetRot;
    private Vector3 axis;
    Vector3[] points = new Vector3[4];

    private bool pause;
    #endregion

    private bool Conclude
    {
        get
        {
            if (Quaternion.Angle(transform.rotation, currentTargetRot) > 1)
            {
                return false;
            }
            return true;
        }
    }

    void Start()
    {
        startRot = transform.rotation;
        endrot = startRot * Quaternion.Euler(rotVector);
        pause = false;
        if (type == RotateType.Reverse)
        {
            rotHandler = ReverceRotate;
            currentTargetRot = endrot;
        }
        else if (type == RotateType.Once)
        {
            rotHandler = ForwardRotate;
            currentTargetRot = startRot;
        }
        else
        {
            axis = transform.right * rotVector.x + transform.up * rotVector.y + transform.forward * rotVector.z;
            rotHandler = RotateAroundAxis; 
        }
    }

    void Update()
    {
        rotHandler();
    }

    public override void Use()
    {
        used = true;
        Invoke("Action", delay);
    }
    public override void ToStart()
    {
        active = false;
        ChangeTarget();
        pause = false;
        transform.rotation = startRot;
        used = false;
    }

    private void ForwardRotate()
    {
        if (active && !pause)
        {
            if (Conclude)
            {
                transform.rotation = currentTargetRot;
                active = false;
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, currentTargetRot, speed * Time.deltaTime);
            }
        }
    }
    private void ReverceRotate()
    {
        if (active && !pause)
        {
            if (Conclude)
            {
                pause = true;
                Invoke("ChangeTarget", pauseTime);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, currentTargetRot, speed * Time.deltaTime);
            }
        }
    }
    private void RotateAroundAxis()
    {
        if(active)
        {
            transform.Rotate(axis, Time.deltaTime * speed);
        }
    }
    private void ChangeTarget()
    {
        if (currentTargetRot == startRot)
        {
            currentTargetRot = endrot;
        }
        else
        {
            currentTargetRot = startRot;
        }
        rotVector *= -1;
        pause = false;
    }
    private void Action()
    {
        if (type == RotateType.Reverse)
        {
            active = !active;
        }
        else
        {
            active = true;
        }
        ChangeTarget();
        pause = false;
    }

    private void OnDrawGizmos()
    {
        if(debug && !(type == RotateType.AroundAxis))
        {
            startRot = transform.rotation;
            endrot = startRot * Quaternion.Euler(rotVector);
            helper.position = transform.position;
            Gizmos.color = Color.cyan;

            currentTargetRot = startRot;
            helper.rotation = startRot;
            points[0] = helper.position + helper.forward * range;

            currentTargetRot = Quaternion.Lerp(startRot, endrot, 0.25f);
            helper.rotation = currentTargetRot;
            points[1] = helper.position + helper.forward * range;

            currentTargetRot = Quaternion.Lerp(startRot, endrot, 0.75f);
            helper.rotation = currentTargetRot;
            points[2] = helper.position + helper.forward * range;

            currentTargetRot = endrot;
            helper.rotation = currentTargetRot;
            points[3] = helper.position + helper.forward * range;

            Gizmos.DrawSphere(points[0], 0.3f);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.cyan, null, 3f);
        }
    }

}
