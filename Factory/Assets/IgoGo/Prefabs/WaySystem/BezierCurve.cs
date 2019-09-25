using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    public BezierPoint prefab;
    [Tooltip("смещение следующей точки")] public Vector3 offset;
    public BezierPoint[] point;
    [Tooltip("конечный результат")] public Vector3[] bezierPath;
    [Tooltip("количество точек, между соседними А и Б (чем больше, тем сильнее сглаживание и выше нагрузка на систему)"), Range(6,100)] public int segmentCount = 25;
    public LineRenderer line;
    public bool iCanMove;

    private BezierPoint last;
    private Vector3 lastPos;
    private int id;

    public void AddPoint() // добавление новой точки
    {
        point = new BezierPoint[transform.childCount + 1];

        for (int j = 0; j < transform.childCount; j++)
        {
            BezierPoint curvePoint = transform.GetChild(j).GetComponent<BezierPoint>();
            if(curvePoint == null)
            {
                curvePoint = transform.GetChild(j).GetChild(0).GetComponent<BezierPoint>();
            }
            point[j] = curvePoint;
        }

        if (last)
        {
            lastPos = last.transform.position;
        }
        last = Instantiate(prefab) as BezierPoint;
        last.gameObject.name = "BezierPoint_" + id;
        if (point.Length > 1)
        {
            last.transform.position = lastPos + transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;
        }
        else
        {
            last.transform.position = transform.position;
        }
        last.transform.parent = transform;
        point[point.Length - 1] = last;

        id++;
    }
    public void ClearAll() // очистка массивов и удаление всех объектов
    {
        for (int i = 0; i < point.Length; i++)
        {
            if (point[i])
            {
                DestroyImmediate(point[i].gameObject);
            }
        }

        point = new BezierPoint[0];
        id = 0;
        DrawCurves();
    }
    public void DestroyLast() // удаление "крайней" точки
    {
        if (last == null) return;

        DestroyImmediate(last.gameObject);

        lastPos -= offset;

        point = new BezierPoint[transform.childCount];
        for (int j = 0; j < transform.childCount; j++)
        {
            point[j] = transform.GetChild(j).GetComponent<BezierPoint>();
            last = point[j];
        }

        id--;
        DrawCurves();
    }

    private void Start()
    {
        point = new BezierPoint[transform.childCount];
        for (int j = 0; j < transform.childCount; j++)
        {
            BezierPoint curvePoint = transform.GetChild(j).GetComponent<BezierPoint>();
            if (curvePoint == null)
            {
                curvePoint = transform.GetChild(j).GetChild(0).GetComponent<BezierPoint>();
            }
            point[j] = curvePoint;
        }
        if (point.Length < 2 || segmentCount < 6) return; // обязательная проверка
        DrawCurves();
    }
    private void LateUpdate()
    {
        if(iCanMove)
        {
            DrawCurves();
        }
    }

    private void OnDrawGizmos()
    {
        if (point.Length < 2 || segmentCount < 6) return; // обязательная проверка
        DrawCurves();
    }

    public void DrawCurves() // создание кривой и визуализация
    {
        bool sw = true;
        List<Vector3> p = new List<Vector3>();
        List<Vector3> l = new List<Vector3>();
        for (int i = 1; i < point.Length; i++)
        {
            if (p.Count == 0)
            {
                p.Add(point[i - 1].endPoint.position);
                p.Add(point[i - 1].adjustPoint.position);
                p.Add(point[i].adjustPoint.position);
            }
            else if (!sw)
            {
                p.Add(point[i - 1].adjustMirror.position);
                p.Add(point[i].adjustMirror.position);
            }
            else
            {
                p.Add(point[i - 1].adjustPoint.position);
                p.Add(point[i].adjustPoint.position);
            }

            p.Add(point[i].endPoint.position);
            sw = !sw;
        }


        for (int i = 0; i < p.Count - 3; i += 3)
        {
            if (l.Count > 0)
            {                               // удаление последней точки кривой, чтобы исключить повтор
                l.RemoveAt(l.Count - 1);
            } 
            for (int j = 0; j <= segmentCount; j++)
            {
                float t = (float)j / segmentCount;
                Vector3 pxl = CalculateBezierPoint(t, p[i], p[i + 1], p[i + 2], p[i + 3]);
                l.Add(pxl);
            }
        }



        bezierPath = new Vector3[] { };
        bezierPath = l.ToArray();
        line.positionCount = bezierPath.Length;
        line.SetPositions(bezierPath);
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
}
    