using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookScript : UsingObject
{
    private Animator anim;
    
    private Vector3 currentDirection;

    private void Start()
    {
        ToStart();
    }

    public override void ToStart()
    {
        anim = GetComponent<Animator>();
        currentDirection = Physics.gravity;
        Use();
    }

    public override void Use()
    {
        if(Vector3.Angle(Physics.gravity, currentDirection) > 90)
        {
            anim.SetTrigger("ChangeSide");
            currentDirection = Physics.gravity;
        }
    }
}
