using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGravModule : UsingObject
{
    public bool planetGravityType;
    public List<Rigidbody> rigidbodies;
    public PlayerStateController player;
    public float radius;

    [Space(20)]
    [Tooltip("Пометить все дочерние объекты как реагирующие на выстрел")] public bool setChilds;
    [Tooltip("Очистить все метки")]public bool clearAllItems;

    private Rigidbody rb;
    private Vector3 gravVector;
    [SerializeField]private List<SphereGravItem> gravItems;
    private int gravMultiplicator;

    private void Start()
    {
        gravMultiplicator = planetGravityType ? 1 : -1;
        rb = GetComponent<Rigidbody>();
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            radius = GetComponent<SphereCollider>().radius; 
        }
    }
    private void FixedUpdate()
    {
        foreach (var item in rigidbodies)
        {
            gravVector = gravMultiplicator * (transform.position - item.position);

            float distance = gravVector.magnitude;
            float strength = 10 * item.mass * rb.mass / (distance * distance);
            item.AddForce(gravVector.normalized * strength);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (setChilds)
        {
            ClearAllItems();
            SetChilds(transform);
            setChilds = false;
        }
        if (clearAllItems)
        {
            ClearAllItems();
            clearAllItems = false;
        }
    }

    public override void Use()
    {
        if(player != null)
        {
            player.playerGravMoveController.SetGravObj(this);
        }
    }
    public override void ToStart()
    {

    }

    private void SetChilds(Transform tr)
    {
        if(tr.childCount > 0)
        {
            for (int i = 0; i < tr.childCount; i++)
            {
                Transform newTr = tr.GetChild(i);
                Collider col = newTr.GetComponentInChildren<Collider>();
                if(col != null && !col.isTrigger)
                {
                    SphereGravItem item = col.GetComponent<SphereGravItem>();
                    if (item == null)
                    {
                        item = col.gameObject.AddComponent<SphereGravItem>();
                        gravItems.Add(item);
                        item.sphere = this;
                    }
                    else
                    {
                        gravItems.Add(item);
                    }
                    if (col.GetComponent<SphereGravModule>() == null)
                    {
                        if (newTr.childCount > 0)
                        {
                            SetChilds(newTr);
                        }
                    }
                }
            }
        }
    }
    private void ClearAllItems()
    {
        if (gravItems != null)
        {
            for (int i = 0; i < gravItems.Count; i++)
            {
                if(gravItems[i] != null)
                gravItems[i].DestroyMe();
            }
            gravItems.Clear();
        }
        else
        {
            gravItems = new List<SphereGravItem>();
        }
        ClearChild(transform);
    }
    private void ClearChild(Transform tr)
    {
        if (tr.childCount > 0)
        {
            for (int i = 0; i < tr.childCount; i++)
            {
                Transform newTr = tr.GetChild(i);
                SphereGravItem item = newTr.gameObject.GetComponent<SphereGravItem>();
                if (item != null)
                {
                    item.DestroyMe();
                }
                if (newTr.childCount > 0)
                {
                    ClearChild(newTr);
                }
            }
        }
    }

   
}
