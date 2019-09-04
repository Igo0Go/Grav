using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    AcidBottle,
    Coin,
    LifeSphere
}


[RequireComponent(typeof(SphereCollider))]
public class LootItem : MonoBehaviour
{
    public ItemType type;
    [Range(1,20)]
    public float speed;
    public int count;

    private int status;
    private Transform target;
    private GravFPS player;


    void Start()
    {
        status = 0;
    }

    public void SetTarget(GravFPS fps)
    {
        target = fps.transform;
        transform.parent = target;
        player = fps;
        status = 1;
    }

    void Update()
    {
        if(status == 1)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                switch(type)
                {
                    case ItemType.AcidBottle:
                        player.gravFPSUI.AddAcid(count);
                        break;
                    case ItemType.Coin:
                        player.gravFPSUI.AddCoin();
                        break;
                    case ItemType.LifeSphere:
                        player.gravFPSUI.AddLifeSphere();
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}
