using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseScript : UsingObject, ITargetTracker
{
    #region Настраиваемые поля
    [SerializeField, Tooltip("Голова коня, из которой происходит бросание лучей")] private Transform head;
    [SerializeField, Tooltip("Сквозь какие слои видит конь")] private LayerMask ignoreMask;
    [SerializeField, Tooltip("Аниматоры индикаторов заряда перед ударом")] private List<Animator> indicators;
    [Space(10), SerializeField, Range(1, 20), Tooltip("Скорость движения при ударе")] private float moveSpeed = 10;
    [SerializeField, Range(1, 20), Tooltip("Скорость поворота на игрока")] private float rotationSpeed = 1;
    [SerializeField, Range(1, 5), Tooltip("Как быстро происходит заряд")] private float reactionSpeed = 1;
    [SerializeField, Range(1, 30), Tooltip("Как сильно отбрасывать игрока")] private float force = 8;
    [SerializeField, Range(1, 30), Tooltip("Урон игроку")] private int damage = 15;
    [Space(10), SerializeField, Range(1, 5), Tooltip("Количество ударов до смерти")] private int health = 2;
    [SerializeField, Tooltip("Части, которые должны развалиться после смерти")] List<Rigidbody> physicalParts;
    #endregion

    #region Служебные подля
    private Animator anim;
    private Vector3 destination;
    private int state;
    #endregion

    public Transform Target => _target;
    private Transform _target;
    private bool NearWithTarget => Vector3.Distance(transform.position, destination) <= moveSpeed * Time.deltaTime * 2;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        foreach (var item in physicalParts)
        {
            item.useGravity = false;
            item.isKinematic = true;
            item.velocity = Vector3.zero;
        }
    }
    private void Update()
    {
        if(_target != null)
        {
            CheckTarget();
            MoveToTarget();
        }
    }

    public override void ToStart()
    {

    }
    public override void Use()
    {
        if (Vector3.Angle(Physics.gravity, -transform.up) > 90)
        {
            //смена гравитации
        }
    }
    public void ClearTarget(Transform target)
    {
        _target = null;
        state = -2;
    }
    public void SetTarget(Transform target)
    {
        _target = target;
        state = 1;
    }

    private void CheckTarget()
    {
        if(state != 0)
        {
            if(state == 1)
            {
                Vector3 currentDirection = _target.position - head.position;
                if (Physics.Raycast(head.position, currentDirection, out RaycastHit hit, currentDirection.magnitude, ~ignoreMask))
                {
                    if (hit.transform == _target)
                    {
                        if (state == 1)
                        {
                            if (ChangeIndicatorsValue(1))
                            {
                                state = 2;
                                GetCurrentTargetPos();
                                transform.LookAt(destination);
                            }
                            RotToTarget();
                        }
                        return;
                    }
                }
                else
                {
                    ChangeIndicatorsValue(0);
                }
            }
            else if(state == 2)
            {
                if (Physics.Raycast(head.position, transform.forward, out RaycastHit hit, 2, ~ignoreMask))
                {
                    if (hit.transform == _target)
                    {
                        _target.GetComponent<GravFPS>().GetDamage(damage);
                        AddForce();
                        state = -1;
                        GetCurrentTargetPos();
                        SetIndicatorsValue(0);
                        return;
                    }
                    else
                    {
                        if(health-1 > 0)
                        {
                            health--;
                            state = -1;
                            GetCurrentTargetPos();
                            return;
                        }
                        else
                        {
                            Dead();
                            return;
                        }
                    }
                }
                if(Vector3.Distance(head.position, _target.position) < 2f)
                {
                    _target.GetComponent<GravFPS>().GetDamage(damage);
                    AddForce();
                    SetIndicatorsValue(0);
                    state = -1;
                    GetCurrentTargetPos();
                    return;
                }
            }
        }
        else
        {
            ChangeIndicatorsValue(0);
        }
    }
    private void MoveToTarget()
    {
        if (state == 2)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if(state == -1)
        {
            Vector3 currentDirection = destination - transform.position;
            if (!NearWithTarget)
            {
                transform.position += currentDirection.normalized * moveSpeed * Time.deltaTime;
            }
            else
            {
                if (ChangeIndicatorsValue(0))
                {
                    state = 1;
                }
            }
        }
    }
    private void RotToTarget()
    {
        Vector3 toTargetDirection = Target.position - transform.position;
        Quaternion stepRot = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(toTargetDirection, transform.up));
        transform.rotation = transform.rotation * stepRot;
    }
    private bool ChangeIndicatorsValue(float value)
    {
        bool result = true;
        foreach (var item in indicators)
        {
            if(value - item.GetFloat("Value") < 0.05f)
            {
                item.SetFloat("Value", value);
            }
            else
            {
                result = false;
                value = Mathf.Lerp(item.GetFloat("Value"), value, Time.deltaTime / reactionSpeed);
                item.SetFloat("Value", value);
            }
        }
        return result;
    }
    private void SetIndicatorsValue(float value)
    {
        foreach (var item in indicators)
        {
            item.SetFloat("Value", value);
        }
    }
    private void Dead()
    {
        state = 0;
        _target = null;
        for (int i = 0; i < indicators.Count; i++)
        {
            Destroy(indicators[i].gameObject, Time.fixedDeltaTime);
        }
        indicators.Clear();
        foreach (var item in physicalParts)
        {
            item.transform.parent = null;
            item.useGravity = true;
            item.isKinematic = false;
            Vector3 forceDir = item.transform.position - head.transform.position;
            item.AddForce(forceDir.normalized * 20, ForceMode.Impulse);
            //Destroy(item.gameObject, 10);
            item.tag = "Manip";
        }
        Destroy(gameObject, 10);
    }
    private void GetCurrentTargetPos()
    {
        if(state == 2)
        {
            destination = _target.position;
        }
        else if(state == -1)
        {
            destination = transform.position - transform.forward * 5;
        }
    }
    private void AddForce()
    {
        if (MyGetComponent(_target.gameObject, out Rigidbody rb))
        {
            Vector3 dir = _target.position - transform.position;
            rb.AddForce(dir.normalized * force, ForceMode.Impulse);
            _target.GetComponent<GravFPS>().Stun();
        }
    }
}
