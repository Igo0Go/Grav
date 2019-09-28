using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DronReplicItem
{
    public string text;
    public AudioClip audioClip;
}

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class FriendScript : MonoBehaviour
{
    [Header("Основные ссылки")]
    [Tooltip("Ссылка на скрипт пушки игрока")] public GravityThrowerScript gravityThrower;
    [Tooltip("Точка, в которую должен возвращаться дрон")] public Transform friendPoint;
    [SerializeField] private GameObject shield;
    [Range(1,10)] public float speed = 5;
    [Range(1, 10)] public float maxShieldTime = 3;


    [Header("Для реплик"), Space(20)]
    [SerializeField] private Color replicasCollor;
    [SerializeField] private GameObject subsPanel;
    [SerializeField] private Text subs;
    [SerializeField] private List<DronReplicItem> actionReplicas;
    [SerializeField] private List<DronReplicItem> altUseReplicas;
    



    private FriendModulePoint modulePoint;
    private AudioSource source;
    private Transform target = null;
    private Rigidbody rb;
    private InputSettingsManager inputSettingsManager;
    private Animator anim;
    private int moveToTarget;
    private float shieldForce;

    public bool NearWithTarget => Vector3.Distance(transform.position, target.position) <= 0.3f;


    void Start()
    {
        gravityThrower.ISeeDronPointEvent += SetTarget;
        gravityThrower.player.OnDeadEvent += ToDead;
        gravityThrower.player.OnRestartEvent += ToRestart;
        inputSettingsManager = gravityThrower.player.inputSettingsManager;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        source = GetComponent<AudioSource>();
        subsPanel.SetActive(false);
        moveToTarget = 0;
        shield.SetActive(false);
        shieldForce = maxShieldTime;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        MoveToTarget();
        ShieldInput();
        ChangeShield();
    }

    public void SetTarget(FriendModulePoint point)
    {
        if(moveToTarget == 0)
        {
            modulePoint = point;
            target = point.friendPoint;
            transform.parent = null;
            moveToTarget = 1;
        }
        StartUseActionReplicas();
    }
    public void ToManipItem()
    {
        transform.parent = null;
    }
    public void ReturnToPoint()
    {
        target = friendPoint;
        moveToTarget = -1;
    }
    public void StartReplicas()
    {
        if (!source.isPlaying)
        {
            int index = 0;
            index = Random.Range(0, altUseReplicas.Count + 4);
            if(index < altUseReplicas.Count)
            {
                subsPanel.SetActive(true);
                source.PlayOneShot(altUseReplicas[index].audioClip);
                subs.text = altUseReplicas[index].text;
                subs.color = replicasCollor;
                Invoke("ReturnSubs", altUseReplicas[index].audioClip.length);
            }
        }
    }
    public void ToDead()
    {
        anim.SetBool("Dead", true);
    }
    public void ToRestart()
    {
        anim.SetBool("Dead", false);
    }

    private void StartUseActionReplicas()
    {
        if (!source.isPlaying)
        {
            int index = 0;
            index = Random.Range(0, actionReplicas.Count + 4);
            if(index < actionReplicas.Count)
            {
                subsPanel.SetActive(true);
                source.PlayOneShot(actionReplicas[index].audioClip);
                subs.text = actionReplicas[index].text;
                subs.color = replicasCollor;
                Invoke("ReturnSubs", actionReplicas[index].audioClip.length);
            }
        }
    }
    private void MoveToTarget()
    {
        if (moveToTarget != 0)
        {
            if (NearWithTarget)
            {
                transform.position = target.position;
                transform.rotation = target.rotation;
                if (moveToTarget == 1)
                {
                    modulePoint.Use();
                    moveToTarget = 2;
                    Invoke("ReturnToPoint", modulePoint.workTime);
                }
                else if (moveToTarget == -1)
                {
                    moveToTarget = 0;
                    transform.parent = target;
                }
                anim.SetFloat("Move", 0, Time.deltaTime * 10, Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
                anim.SetFloat("Move", 1, Time.deltaTime * 10, Time.deltaTime);
                transform.LookAt(target.position);
            }
        }
        else
        {
            anim.SetFloat("Move", 0, Time.deltaTime * 10, Time.deltaTime);
        }
    }
    private void ShieldInput()
    {
        if(Input.GetKeyDown(inputSettingsManager.GetKey("ChangeUsing")) && moveToTarget == 0)
        {
            UseShield();
        }
    }
    private void UseShield()
    {
        if(!shield.activeSelf)
        {
            if (shieldForce > 0.5f)
            {
                shield.SetActive(true);
                StartReplicas();
            }
        }
        else
        {
            shield.SetActive(false);
        }
    }
    private void ChangeShield()
    {
        if(shield.activeSelf)
        {
            if(shieldForce > 0)
            {
                shieldForce -= Time.deltaTime;
            }
            else
            {
                shieldForce = 0;
                shield.SetActive(false);
            }
        }
        else
        {
            if (shieldForce <= maxShieldTime)
            {
                shieldForce += Time.deltaTime;
            }
            else
            {
                shieldForce = maxShieldTime;
            }
        }
    }
    private void ReturnSubs() => subsPanel.SetActive(false);
}
