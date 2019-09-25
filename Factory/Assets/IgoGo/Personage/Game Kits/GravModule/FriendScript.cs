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
    public Transform friendSystemBufer;
    [Tooltip("Точка, в которую должен возвращаться дрон")] public Transform friendPoint;
    [Range(1,10)] public float speed = 5;
    [Tooltip("Если активно, работает ЗАЯ")] public bool activeState;

    [Header("Для реплик")]
    [SerializeField] private Color actionColor;
    [SerializeField] private Color passinveColor;
    [SerializeField] private GameObject subsPanel;
    [SerializeField] private Text subs;
    [SerializeField] private List<DronReplicItem> actionReplicas;
    [SerializeField] private List<DronReplicItem> passiveReplicas;
    [SerializeField] private List<DronReplicItem> actionUseReplicas;
    [SerializeField] private List<DronReplicItem> passiveUseReplicas;



    private FriendModulePoint modulePoint;
    private AudioSource source;
    private Transform target = null;
    private Rigidbody rb;
    private InputSettingsManager inputSettingsManager;
    private Transform player;
    private int moveToTarget;

    public bool NearWithTarget => Vector3.Distance(transform.position, target.position) <= 0.3f;


    void Start()
    {
        gravityThrower.ISeeDronPointEvent += SetTarget;
        gravityThrower.player.RotEvent += CheckRotate;
        inputSettingsManager = gravityThrower.player.inputSettingsManager;
        player = gravityThrower.player.transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        source = GetComponent<AudioSource>();
        subsPanel.SetActive(false);
        moveToTarget = 0;
    }

    void Update()
    {
        MoveToTarget();
        ChangeState();
        CheckSystemOffset();
    }

    public void CheckSystemOffset()
    {
        friendSystemBufer.transform.position = player.transform.position;
    }
    
    public void SetTarget(FriendModulePoint point)
    {
        if(activeState)
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
            subsPanel.SetActive(true);
            if (activeState)
            {
                index = Random.Range(0, actionReplicas.Count);
                source.PlayOneShot(actionReplicas[index].audioClip);
                subs.text = actionReplicas[index].text;
                subs.color = actionColor;
                Invoke("ReturnSubs", actionReplicas[index].audioClip.length);
            }
            else
            {
                index = Random.Range(0, passiveReplicas.Count);
                source.PlayOneShot(passiveReplicas[index].audioClip);
                subs.text = passiveReplicas[index].text;
                subs.color = passinveColor;
                Invoke("ReturnSubs", passiveReplicas[index].audioClip.length);
            }
        }
    }

    private void StartUseActionReplicas()
    {
        if (!source.isPlaying)
        {
            int index = 0;
            subsPanel.SetActive(true);
            if (activeState)
            {
                index = Random.Range(0, actionUseReplicas.Count);
                source.PlayOneShot(actionUseReplicas[index].audioClip);
                subs.text = actionUseReplicas[index].text;
                subs.color = actionColor;
                Invoke("ReturnSubs", actionUseReplicas[index].audioClip.length);
            }
            else
            {
                index = Random.Range(0, passiveUseReplicas.Count);
                source.PlayOneShot(passiveUseReplicas[index].audioClip);
                subs.text = passiveUseReplicas[index].text;
                subs.color = passinveColor;
                Invoke("ReturnSubs", passiveUseReplicas[index].audioClip.length);
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
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
                transform.LookAt(target.position);
            }
        }
    }
    private void ChangeState()
    {
        if(Input.GetKeyDown(inputSettingsManager.GetKey("ChangeUsing")) && moveToTarget == 0)
        {
            activeState = !activeState;
        }
    }
    private void CheckRotate(Quaternion rot)
    {
        friendSystemBufer.rotation = rot * friendSystemBufer.rotation;
    }
    private void ReturnSubs() => subsPanel.SetActive(false);
}
