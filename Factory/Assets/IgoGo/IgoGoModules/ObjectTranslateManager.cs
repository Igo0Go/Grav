using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TranslateType
{
    Reverce,
    Once,
    Loop
}
public class ObjectTranslateManager : UsingObject {

    [Tooltip("Смещения. Суммируются от начальной позиции. При старте объект сместиться от начального состояния на вектор из элемента 0," +
        "потом от полученной позции ещё на вектор из элемента 1 и т.д.")]
    public List<Vector3> offsetPos;
    [Tooltip("скорость движения")] public float speed;
    [Tooltip("Задержка перед запуском")] public float delay;
    [Tooltip("Задержка между циклами")] public float pauseTime;
    [Tooltip("Тип перемещения")] public TranslateType type = TranslateType.Once;

    private Action moveHandler;
    private Vector3[] debugPoints = new Vector3[8];
    private Vector3 currentTargetPos;
    private Vector3 moveVector;
    private Vector3 startPos;
    [Space(20)] public bool active;
    private bool pause;
    private int currentOffsetItem;
    private int forwardWay;

    private bool Conclude
    {
        get
        {
            if (Vector3.Distance(transform.position, currentTargetPos) > speed * Time.deltaTime)
            {
                return false;
            }
            return true;
        }
    }

    void Start()
    {
        pause = false;
        startPos = transform.position;
        if (type == TranslateType.Once)
        {
            forwardWay = -1;
            currentTargetPos = transform.position;
            moveHandler = ForwardMove;
        }
        else
        {
            currentOffsetItem = 0;
            forwardWay = 1;
            currentTargetPos = transform.position + forwardWay * (transform.right * offsetPos[currentOffsetItem].x + transform.up * offsetPos[currentOffsetItem].y
                + transform.forward * offsetPos[currentOffsetItem].z);
            moveVector = currentTargetPos - transform.position;
            moveVector = moveVector.normalized;

            if (type == TranslateType.Reverce)
            {
                moveHandler = ReverceMove;
            }
            else
            {
                moveHandler = LoopMove;
               
            }
            if (active)
            {
                active = false;
                Invoke("Action", delay);
            }
        }
    }
    void Update()
    {
        moveHandler();
    }

    public override void Use()
    {
        Invoke("Action", delay);
        used = true;
    }
    public override void ToStart()
    {
        currentOffsetItem = 0;
        transform.position = startPos;
        pause = false;
        if (type == TranslateType.Once)
        {
            forwardWay = -1;
            currentTargetPos = transform.position;
        }
        else
        {
            currentOffsetItem = 0;
            forwardWay = 1;
            currentTargetPos = transform.position + forwardWay * (transform.right * offsetPos[currentOffsetItem].x + transform.up * offsetPos[currentOffsetItem].y
                + transform.forward * offsetPos[currentOffsetItem].z);
            moveVector = currentTargetPos - transform.position;
            moveVector = moveVector.normalized;

            if (active)
            {
                active = false;
                Invoke("Action", delay);
            }
        }
        used = false;
    }


    private void ForwardMove()
    {
        if (active && !pause)
        {
            if (Conclude)
            {
                transform.position = currentTargetPos;
                if ((forwardWay == 1 && currentOffsetItem == offsetPos.Count - 1) || (forwardWay == -1 && currentOffsetItem == 0))
                {
                    active = false;
                }
                else
                {
                    pause = true;
                    Invoke("ChangeTarget", pauseTime);
                }
            }
            else
            {
                transform.position += moveVector * speed * Time.deltaTime;
            }
        }
    }
    private void LoopMove()
    {
        if (active && !pause)
        {
            if (Conclude)
            {
                transform.position = currentTargetPos;
                pause = true;
                if(currentOffsetItem == offsetPos.Count-1)
                {
                    currentOffsetItem = 0;
                    transform.position = startPos;
                    pause = false;
                    currentOffsetItem = 0;
                    forwardWay = 1;
                    currentTargetPos = transform.position + forwardWay * (transform.right * offsetPos[currentOffsetItem].x + transform.up * offsetPos[currentOffsetItem].y
                        + transform.forward * offsetPos[currentOffsetItem].z);
                    moveVector = currentTargetPos - transform.position;
                    moveVector = moveVector.normalized;
                }
                else
                {
                    Invoke("ChangeTarget", pauseTime);
                }
            }
            else
            {
                transform.position += moveVector * speed * Time.deltaTime;
            }
        }
    }
    private void ReverceMove()
    {
        if (active && !pause)
        {
            if (Conclude)
            {
                transform.position = currentTargetPos;
                pause = true;
                Invoke("ChangeTarget", pauseTime);
            }
            else
            {
                transform.position += moveVector * speed * Time.deltaTime;
            }
        }
    }
    private void RestartForwardMove()
    {
        forwardWay *= -1;

        if (forwardWay == 1)
        {
            currentOffsetItem++;
            if (currentOffsetItem == offsetPos.Count)
            {
                return;
            }
        }
        else
        {
            currentOffsetItem--;
            if (currentOffsetItem < 0)
            {
                return;
            }
        }

        Vector3 oldPos = currentTargetPos;
        currentTargetPos = oldPos + (forwardWay * (transform.right * offsetPos[currentOffsetItem].x +
            transform.up * offsetPos[currentOffsetItem].y + transform.forward * offsetPos[currentOffsetItem].z));

        moveVector = currentTargetPos - transform.position;
        moveVector = moveVector.normalized;
        pause = false;
    }
    private void ChangeTarget()
    {
        if (forwardWay == 1)
        {
            currentOffsetItem++;
            if (currentOffsetItem == offsetPos.Count)
            {
                currentOffsetItem--;
                forwardWay = -1;
            }
        }
        else
        {
            currentOffsetItem--;
            if (currentOffsetItem < 0)
            {
                currentOffsetItem++;
                forwardWay = 1;
            }
        }
        Vector3 oldPos = currentTargetPos;
        currentTargetPos = oldPos + (forwardWay * (transform.right * offsetPos[currentOffsetItem].x +
            transform.up * offsetPos[currentOffsetItem].y + transform.forward * offsetPos[currentOffsetItem].z));

        moveVector = currentTargetPos - transform.position;
        moveVector = moveVector.normalized;
        pause = false;
    }
    private void Action()
    {
        if (type == TranslateType.Once)
        {
            ChangeTarget();
            active = true;
        }
        else
        {
            active = !active;
        }
        pause = false;
    }
    private void OnDrawGizmos()
    {
        if(debug)
        {
            Gizmos.color = Color.cyan;
            Vector3 bufer1 = transform.position;
            Vector3 bufer2;
            if(offsetPos != null)
            {
                for (int i = 0; i < offsetPos.Count; i++)
                {
                    bufer2 = bufer1 + transform.right * offsetPos[i].x + transform.up * offsetPos[i].y + transform.forward * offsetPos[i].z;
                    Gizmos.DrawLine(bufer1, bufer2);
                    bufer1 = bufer2;
                    Gizmos.DrawSphere(bufer1, 0.4f);
                    if (i == offsetPos.Count - 1)
                    {
                        #region Подготовка точек
                        debugPoints[0] = bufer1 - (transform.forward * transform.lossyScale.z / 2) - (transform.right * transform.lossyScale.x / 2)
                                       - (transform.up * transform.lossyScale.y / 2);
                        debugPoints[1] = bufer1 - (transform.forward * transform.lossyScale.z / 2) + (transform.right * transform.lossyScale.x / 2)
                            - (transform.up * transform.lossyScale.y / 2);
                        debugPoints[2] = bufer1 + (transform.forward * transform.lossyScale.z / 2) + (transform.right * transform.lossyScale.x / 2)
                            - (transform.up * transform.lossyScale.y / 2);
                        debugPoints[3] = bufer1 + (transform.forward * transform.lossyScale.z / 2) - (transform.right * transform.lossyScale.x / 2)
                            - (transform.up * transform.lossyScale.y / 2);
                        debugPoints[4] = bufer1 + (transform.forward * transform.lossyScale.z / 2) - (transform.right * transform.lossyScale.x / 2)
                            + (transform.up * transform.lossyScale.y / 2);
                        debugPoints[5] = bufer1 - (transform.forward * transform.lossyScale.z / 2) - (transform.right * transform.lossyScale.x / 2)
                            + (transform.up * transform.lossyScale.y / 2);
                        debugPoints[6] = bufer1 - (transform.forward * transform.lossyScale.z / 2) + (transform.right * transform.lossyScale.x / 2)
                            + (transform.up * transform.lossyScale.y / 2);
                        debugPoints[7] = bufer1 + (transform.forward * transform.lossyScale.z / 2) + (transform.right * transform.lossyScale.x / 2)
                            + (transform.up * transform.lossyScale.y / 2);
                        #endregion

                        for (int j = 0; j < debugPoints.Length - 1; j++)
                        {
                            Gizmos.DrawLine(debugPoints[j], debugPoints[j + 1]);
                        }
                        Gizmos.DrawLine(debugPoints[0], debugPoints[5]);
                        Gizmos.DrawLine(debugPoints[4], debugPoints[7]);
                        Gizmos.DrawLine(debugPoints[2], debugPoints[7]);
                        Gizmos.DrawLine(debugPoints[1], debugPoints[6]);
                        Gizmos.DrawLine(debugPoints[0], debugPoints[3]);
                    }
                }
            }
        }
    }
}
