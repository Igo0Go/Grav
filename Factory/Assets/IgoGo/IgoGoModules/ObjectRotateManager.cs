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
    #region Настраиваемые поля
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
    [Tooltip("Дальность линии от центра"), Range(1, 10)] public float range = 1;
    #endregion

    #region Служебные
    private Action rotHandler;
    private Quaternion startRot;
    private Quaternion endRot;
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
        endRot = startRot * Quaternion.Euler(rotVector);
        pause = false;
        if (type == RotateType.Reverse)
        {
            rotHandler = ReverceRotate;
            currentTargetRot = endRot;
        }
        else if (type == RotateType.Once)
        {
            rotHandler = ForwardRotate;
            currentTargetRot = startRot;
        }
        else
        {
            axis = rotVector;
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
        if (active)
        {
            Quaternion rot = Quaternion.AngleAxis(Time.deltaTime * speed, axis);
            transform.rotation = transform.rotation * rot;
        }
    }
    private void ChangeTarget()
    {
        if (currentTargetRot == startRot)
        {
            currentTargetRot = endRot;
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (debug)
        {
            if (type != RotateType.AroundAxis)
            {
                if (helper == null)
                {
                    helper = Instantiate(new GameObject(), transform).transform;
                    helper.name = "helper";
                }
                startRot = transform.rotation;
                endRot = startRot * Quaternion.Euler(rotVector);
                helper.position = transform.position;
                Gizmos.color = Color.cyan;

                currentTargetRot = startRot;
                helper.rotation = startRot;
                points[0] = helper.position + helper.forward * range;

                currentTargetRot = Quaternion.Lerp(startRot, endRot, 0.25f);
                helper.rotation = currentTargetRot;
                points[1] = helper.position + helper.forward * range;

                currentTargetRot = Quaternion.Lerp(startRot, endRot, 0.75f);
                helper.rotation = currentTargetRot;
                points[2] = helper.position + helper.forward * range;

                currentTargetRot = endRot;
                helper.rotation = currentTargetRot;
                points[3] = helper.position + helper.forward * range;

                Gizmos.DrawSphere(points[0], 0.3f);
                Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.cyan, null, 3f);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Vector3 offset = transform.right * rotVector.x + transform.up * rotVector.y + transform.forward * rotVector.z;
                Gizmos.DrawSphere(transform.position - offset.normalized * range, 0.3f);
                Gizmos.DrawSphere(transform.position + offset.normalized * range, 0.3f);
                Gizmos.DrawLine(transform.position - offset.normalized * range, transform.position + offset.normalized * range);
            }
        }
    }
#endif
}

