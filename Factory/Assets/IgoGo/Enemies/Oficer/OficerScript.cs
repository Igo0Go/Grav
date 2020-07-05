using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OficerScript : MyTools, ITargetTracker
{
    [Tooltip("Та чать, которая будет бить по игроку")] public Transform body;
    [Tooltip("Точка, в которую нужно возвращаться body после удара")] public Transform defaultBodyPoint;
    [Tooltip("Сквозь какие слои видит слон")] public LayerMask ignoreMask;
    [Range(1, 40), Tooltip("Скорость удара")] public float speed = 8;
    [Range(1, 30), Tooltip("Время между ударами")] public float reloadTime = 3;
    [Range(1, 30), Tooltip("Сила отбрасывания при появлении")] public float force = 8;
    [Range(1, 30), Tooltip("Урон при ударе")] public int damage = 15;

    private Animator anim;
    private Transform _target;
    private bool toTarget;
    private bool active;
    private int attack;

    public Transform Target => toTarget ? _target : defaultBodyPoint;
    private bool NearWithTarget => Vector3.Distance(body.position, Target.position) <= speed * Time.deltaTime * 2;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        CheckTarget();
        MoveToTarget();
    }

    public void ClearTarget(Transform target)
    {
        _target = null;
        attack = -2;
        toTarget = false;
    }
    public void SetTarget(Transform target)
    {
        _target = target;
        anim.SetBool("Active", true);
        Invoke("AddForce", 1);
    }
    public void Activation()
    {
        if(_target != null)
        {
            active = true;
            attack = 1;
        }
    }

    private void CheckTarget()
    {
        if(active)
        {
            if (attack > 0)
            {
                Vector3 currentDirection = _target.position - body.position;
                if (Physics.Raycast(body.position, currentDirection, out RaycastHit hit, currentDirection.magnitude, ~ignoreMask))
                {
                    if (hit.transform == _target)
                    {
                        if (attack == 1)
                        {
                            attack = 2;
                        }
                        toTarget = true;
                        return;
                    }
                }
                if (attack != 2)
                {
                    toTarget = false;
                }
            }
            else
            {
                toTarget = false;
            }
        }
    }
    private void MoveToTarget()
    {
        if (attack != 0)
        {
            Vector3 currentDirection = Target.position - body.position;
            if (NearWithTarget)
            {
                body.position = Target.position;
                if (attack == 2)
                {
                    if(MyGetComponent(_target.gameObject, out PlayerReactionsController reactionController))
                    {
                        reactionController.GetDamage(damage);     
                    }
                    attack = -1;
                }
                else if (attack == -1)
                {
                    attack = 0;
                    Invoke("ReturnAttack", reloadTime);
                }
                else if (attack == -2)
                {
                    attack = 0;
                    active = false;
                    anim.SetBool("Active", active);
                }
            }
            else
            {
                body.position += currentDirection.normalized * speed * Time.deltaTime;
            }
        }
    }
    private void AddForce()
    {
        if (anim.GetBool("Active") && MyGetComponent(_target.gameObject, out Rigidbody rb))
        {
            Vector3 dir = _target.position - transform.position;
            rb.AddForce(dir.normalized * force, ForceMode.Impulse);
            _target.GetComponent<PlayerStateController>().Stun();
        }
    }
    private void ReturnAttack() => attack = 1;
}
