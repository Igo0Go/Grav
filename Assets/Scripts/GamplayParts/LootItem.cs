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
    public Collider physicalCollider;
    public Rigidbody rb;
    [Range(0.1f, 5)] public float activationTime = 1;
    public AudioSource source;

    private int status;
    private Transform target;
    private PlayerStateController player;

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
                        player.playerUIController.AddAcid(count);
                        break;
                    case ItemType.Coin:
                        player.playerUIController.AddCoin();
                        break;
                    case ItemType.LifeSphere:
                        player.playerUIController.AddLifeSphere();
                        break;
                    case ItemType.BankCard:
                        player.playerUIController.AddBankCard(count);
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

    public void SetTarget(PlayerStateController playerState)
    {
        target = playerState.transform;
        //transform.parent = target;
        player = playerState;
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
