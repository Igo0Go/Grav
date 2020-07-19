using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Pawn : UsingObject, ITargetTracker 
{
    #region Настраиваемые поля
    [Tooltip("Префаб манеты, которая падает")] public GameObject coinWithRb;
    [Range(1, 15), Tooltip("Радиус определения новой точки для перемещения")] public float destinationRange = 1;
    [Range(1, 5), Tooltip("Сила бросания предметов")] public float manipShootForce = 1;
    [Tooltip("Сквозь какие слои видит пешка")] public LayerMask ignoreMask;
    [Tooltip("К какой части притягиваются предметы")] public Transform manipPoint;
    #endregion

    #region Служебные поля
    private List<Transform> targetPoints;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Rigidbody rbBufer;
    private PlayerStateController gravFPS;
    private Collider viewTrigger;
    /// <summary>
    /// 0 выбор пункта, 1 идти до пункта, -1 идти к ящику, -2 ждать ящик, 2 идти до игрока, 3 выбить деньги, 4 - нас взяли
    /// </summary>
    private sbyte state;
    #endregion

    #region Свойства
    public Transform Target => _target;
    private Transform _target;
    private bool NearWithDestination => Vector3.Distance(transform.position, agent.destination) < 1;
    private bool NearWithTarget => Vector3.Distance(transform.position, _target.position) < 1;
    #endregion

    #region Обработка событий Unity
    void Start()
    {
        viewTrigger = GetComponent<Collider>();
        targetPoints = new List<Transform>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        CheckDestination();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Invoke("ReturnDefault", 1f);
    }
    #endregion

    #region Публичные методы (реакция на внешние раздражители)
    public void ClearTarget(Transform target)
    {
        targetPoints.Remove(target);
        if(target == _target)
        {
            _target = null;
            GetNewTarget();
        }
       
    }
    public void SetTarget(Transform target)
    {
        if(!targetPoints.Contains(target))
        {
            targetPoints.Add(target);
        }
        GetNewTarget();
    }
    public override void ToStart()
    {

    }
    public override void Use()
    {

    }
    public void ToManipState()
    {
        state = 4;
        agent.isStopped = true;
        agent.enabled = false;
        rb.useGravity = false;
        rb.isKinematic = false;
        viewTrigger.enabled = false;
    }
    public void ToDefaultState()
    {
        state = 5;
    }
    #endregion

    #region Служебные методы (поведение)
    private void CheckDestination()
    {
        switch (state)
        {
            case -2:
                ManipTarget();
                break;
            case -1:
                CheckDistance();
                break;
            case 0:
                GetRandomDestination();
                break;
            case 2:
                CheckDistance();
                break;
            case 3:
                GetMony();
                break;
        }
    }
    private void GetRandomDestination()
    {
        if(agent.enabled)
        {
            Vector3 origin = GetRandomOrigin();
            origin += transform.up * 5;

            RaycastHit hit;
            if (Physics.Raycast(origin, -transform.up, out hit, 100, ~ignoreMask))
            {
                agent.destination = hit.point;
                if (agent.hasPath)
                {
                    agent.isStopped = false;
                    state = 1;
                }
                else
                {
                    agent.isStopped = true;
                }
            }
        }
    }
    private void ManipTarget()
    {
        if(NearWithTarget)
        {
            ShootManip();
        }
        else
        {
            Vector3 direction = _target.position - manipPoint.position;
            rbBufer.velocity = Vector3.zero;
            rbBufer.AddForce(direction.normalized * manipShootForce, ForceMode.Impulse);
        }
    }
    private void ShootManip()
    {
        rbBufer.isKinematic = false;
        rbBufer.useGravity = true;
        rbBufer.velocity = Vector3.zero;
        Vector3 direction = transform.forward * Random.Range(-1, 1) + transform.right * Random.Range(-1, 1) + transform.up;
        rbBufer.AddForce(direction.normalized * manipShootForce * 3, ForceMode.Impulse);
        rbBufer = null;
        GetNewTarget();
    }
    private void GetMony()
    {
        gravFPS.playerUIController.Spend(1);
        rbBufer = Instantiate(coinWithRb, manipPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        Vector3 direction = Vector3.ProjectOnPlane(_target.position - manipPoint.position, transform.up) + transform.up;
        rbBufer.AddForce(direction.normalized * manipShootForce * 2, ForceMode.Impulse);
        GetNewTarget();
    }
    private Vector3 GetRandomOrigin()
    {
        Vector3 result = transform.position;
        result = result + transform.forward * Random.Range(-destinationRange, destinationRange) +
            transform.right * Random.Range(-destinationRange, destinationRange); 
        return result;
    }
    private void CheckDistance()
    {
        if(state == 1)
        {
            if (NearWithDestination)
            {
                state = 0;
                agent.isStopped = true;
            }
            return;
        }
        else
        {
            agent.destination = _target.position;
            if (NearWithTarget)
            {
                if (state == -1)
                {
                    state = -2;
                }
                else if (state == 2)
                {
                    state = 3;
                }
                return;
            }
           
        }
    }
    private void GetNewTarget()
    {
        if (_target != null)
        {
            targetPoints.Remove(_target);
        }
        if (targetPoints.Count > 0)
        {
            if (MyGetComponent(targetPoints[0].gameObject, out ManipItem manip))
            {
                rbBufer = manip.GetComponent<Rigidbody>();
                state = -1;
                agent.destination = targetPoints[0].position;
                agent.isStopped = false;
            }
            else if (MyGetComponent(targetPoints[0].gameObject, out gravFPS))
            {
                agent.destination = targetPoints[0].position;
                agent.isStopped = false;
                state = 2;
            }
            if(_target != null)
            {
                targetPoints.Add(_target);
            }
            _target = targetPoints[0];
            return;
        }
        if(_target != null)
        {
            targetPoints.Add(_target);
        }
        state = 0;
    }
    private void ReturnDefault()
    {
        if (state == 5)
        {
            agent.enabled = true;
            transform.up = Vector3.up;
            rb.useGravity = false;
            rb.isKinematic = true;
            viewTrigger.enabled = true;
            GetNewTarget();
        }
    }
    #endregion
}
