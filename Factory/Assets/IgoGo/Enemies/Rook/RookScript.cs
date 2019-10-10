using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookScript : UsingObject
{
    #region Настраиваемые поля
    [SerializeField, Tooltip("Верхняя точка остановки центральной части")] private Transform posUp;
    [SerializeField, Tooltip("Нижняя точка остановки центральной части")] private Transform posDown;
    [SerializeField, Tooltip("Центральная часть ладьи, которая будет перемещаться")] private Transform body;
    [SerializeField, Tooltip("Аниматор центральной части")] private Animator anim;
    [SerializeField, Tooltip("Начальное положение центральной части")] private bool toUp;
    [SerializeField, Range(0.1f, 20), Tooltip("Скорость смены положения при изменении гравитации")] private float changeSpeed = 1;
    #endregion

    #region Служебные поля
    private Vector3 currentDirection;
    private Transform currentTarget;
    private bool move;
    #endregion

    private bool NearWithTarget => Vector3.Distance(body.position, currentTarget.position) <= changeSpeed * Time.deltaTime * 2;

    private void Start()
    {
        ToStart();
    }
    private void Update()
    {
        MoveToTarget();
    }

    public override void ToStart()
    {
        if (!toUp)
        {
            currentTarget = posUp;
        }
        else
        {
            currentTarget = posDown;
        }
        currentDirection = currentTarget.position - body.position;
        anim.SetBool("Side", toUp);
        move = false;
        Use();
    }
    public override void Use()
    {
        if(Vector3.Angle(Physics.gravity, currentDirection) > 90)
        {
            ChangeTarget();
        }
    }

    private void ChangeTarget()
    {
        currentTarget = currentTarget == posUp ? posDown : posUp;
        currentDirection = currentTarget.position - body.position;
        move = true;
    }
    private void MoveToTarget()
    {
        if(move)
        {
            if (NearWithTarget)
            {
                body.position = currentTarget.position;
                anim.SetBool("Side", !anim.GetBool("Side"));
                move = false;
            }
            else
            {
                body.position += currentDirection.normalized * changeSpeed * Time.deltaTime;
            }
        }
    }
}
