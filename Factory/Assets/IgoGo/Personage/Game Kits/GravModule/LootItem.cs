using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    AcidBottle,
    Coin,
    LifeSphere,
    BankCard
}


[RequireComponent(typeof(SphereCollider))]
public class LootItem : MonoBehaviour
{
    public ItemType type;
    [Range(1,20)]
    public float speed;
    public int count;
    public bool opportunityToSuffice;
    [SerializeField] private Collider physicalCollider;
    [SerializeField] private Rigidbody rb;
    [SerializeField, Range(0.1f, 5)] private float activationTime = 1;
    public AudioSource source;

    private int status;
    private Transform target;
    private GravFPS player;

    void Update()
    {
        if (status == 1)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                switch (type)
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
                    case ItemType.BankCard:
                        player.gravFPSUI.AddBankCard(count);
                        break;
                }
                source.Play();
                Destroy(gameObject, source.clip.length);
                status = 0;
            }
        }
    }
    void Start()
    {
        status = 0;
        if(!opportunityToSuffice)
        {
            Invoke("Activate", activationTime);
        }
    }

    public void SetTarget(GravFPS fps)
    {
        target = fps.transform;
        transform.parent = target;
        player = fps;
        status = 1;
        if(physicalCollider != null)
        {
            Destroy(physicalCollider);
        }
        if(rb != null)
        {
            Destroy(rb);
        }
    }

    private void Activate() => opportunityToSuffice = true;

}
