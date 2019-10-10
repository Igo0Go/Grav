using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OficerScript : MyTools, ITargetTracker
{
    [SerializeField, Tooltip("Та чать, которая будет бить по игроку")] private Transform body;
    [SerializeField, Tooltip("Точка, в которую нужно возвращаться body после удара")] private Transform defaultBodyPoint;
    [SerializeField, Tooltip("Сквозь какие слои видит слон")] private LayerMask ignoreMask;
    [SerializeField, Range(1, 40), Tooltip("Скорость удара")] private float speed = 8;
    [SerializeField, Range(1, 30), Tooltip("Время между ударами")] private float reloadTime = 3;
    [SerializeField, Range(1, 30), Tooltip("Сила отбрасывания при появлении")] private float force = 8;
    [SerializeField, Range(1, 30), Tooltip("Урон при ударе")] private int damage = 15;

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
                    if(MyGetComponent(_target.gameObject, out GravFPS gravFPS))
                    {
                        gravFPS.GetDamage(damage);     
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
            _target.GetComponent<GravFPS>().Stun();
        }
    }
    private void ReturnAttack() => attack = 1;
}
