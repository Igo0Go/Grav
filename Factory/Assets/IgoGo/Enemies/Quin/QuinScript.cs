using UnityEngine;

public class QuinScript : UsingObject, ITargetTracker
{
    [Tooltip("Та часть, которая будет преследовать игрока")] public Transform body;
    [Tooltip("Точка, в которую будет уходить ферзь, если его дразнит кролик")] public Transform falePoint;
    [Tooltip("Сквозь какие слои видеит ферзь")] public LayerMask ignoreMask;
    [Range(0.1f, 20)] public float speed = 8;
    public Animator anim;
    
    private Transform _target;
    private bool toTarget;
    private int attack;

    public Transform Target => toTarget?  _target : transform;
    private bool NearWithTarget => Vector3.Distance(body.position, Target.position) <= speed * Time.deltaTime * 2;

    void Update()
    {
        CheckTarget();
        MoveToTarget();
    }

    public void ClearTarget()
    {
        _target = null;
        attack = -1;
        toTarget = false;
    }
    public void ClearTarget(Transform target)
    {
        _target = null;
        attack = -1;
        toTarget = false;
    }
    public void SetTarget(Transform target)
    {
        _target = target;
        attack = 1;
    }

    private void CheckTarget()
    {
        if(attack > 0)
        {
            Vector3 currentDirection = _target.position - body.position;
            if (Physics.Raycast(body.position, currentDirection, out RaycastHit hit, currentDirection.magnitude, ~ignoreMask))
            {
                if (hit.transform == _target)
                {
                    if(attack == 1)
                    {
                        anim.SetTrigger("Attack");
                        attack = 2;
                    }
                    toTarget = true;
                    return;
                }
            }
            if(attack != 2)
            {
                toTarget = false;
            }
        }
    }
    private void MoveToTarget()
    {
        if (attack != 0)
        {
            Vector3 currentDirection = (Target.position + Target.up) - body.position;
            body.forward = currentDirection;
            if (NearWithTarget)
            {
                body.position = Target.position;
                if(attack == 2)
                {
                    attack = -1;
                }
                else if(attack == -1)
                {
                    attack = 0;
                }
            }
            else
            {
                body.position += currentDirection.normalized * speed * Time.deltaTime;
            }
        }
    }

    public override void Use()
    {
        SetTarget(falePoint);
        Invoke("ClearTarget", 1);
    }
    public override void ToStart()
    {

    }
}
