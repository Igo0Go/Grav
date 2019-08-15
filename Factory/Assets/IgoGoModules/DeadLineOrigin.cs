using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DeadLineOrigin : MonoBehaviour
{
    [Tooltip("На какие слои не будет реагировать луч")]public LayerMask ignoreMask;
    [Tooltip("Пак частиц, которые появляются в месте, куда бъёт луч.")] public GameObject sparks;
    [Tooltip("Максимальная дальность луча"), Range(1, 1000)] public float maxRange = 10;


    private LineRenderer renderItem;
    void Start()
    {
        renderItem = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DeadRay();
    }

    private void DeadRay()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxRange, ~ignoreMask))
        {
            if(hit.collider.tag.Equals("Player"))
            {
                hit.collider.GetComponent<GravFPS>().GetDamage(5);
            }
            DrawDeadLine(hit.point);
            GameObject bufer = Instantiate(sparks, hit.point + hit.normal * 0.01f, Quaternion.identity, hit.transform);
            bufer.transform.up = hit.normal;
            Destroy(bufer, 0.3f);
        }
        else
        {
            DrawDeadLine(transform.position + transform.forward * 1000);
        }
    }

    private void DrawDeadLine(Vector3 point)
    {
        renderItem.positionCount = 2;
        renderItem.SetPosition(0, transform.position);
        renderItem.SetPosition(1, point);
    }
}
