using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookScript : UsingObject
{
    [SerializeField] private Transform posUp;
    [SerializeField] private Transform posDown;
    [SerializeField] private Transform body;
    [SerializeField] private Animator anim;
    [SerializeField] private bool toUp;
    [SerializeField, Range(0.1f, 20)] private float changeSpeed = 1;
    
    private Vector3 currentDirection;
    private Transform currentTarget;
    private bool move;

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
