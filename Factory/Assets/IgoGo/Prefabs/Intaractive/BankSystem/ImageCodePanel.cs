using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCodePanel : UsingOrigin
{
    public Transform playerPos;
    public Collider triger;
    [Range(1, 3)] public float moveSpeed;

    [Space(10)] public List<Transform> pointsForInput;
    public List<GameObject> prefabs;
    public GameObject activeToggle;

    [Space(10), Range(1, 10)] public int attemptsCount;
    public string code;
    public bool active;
    public int coinsCount;

    [Space(10)] public AudioClip messageClip;
    public string messageText;
    public GameObject subsPanel;
    public Text subsText;
    [Space(20)] public string currentCode;

    private List<GameObject> currentCodeImages;
    private GravFPS gravFPS;
    private Transform player;
    private Transform playerCam;
    private AudioSource source;
    private int currentAttempt;
    private bool move;
    

    private bool PlayerNearWithTarget => Vector3.Distance(player.position, playerPos.position) < moveSpeed * Time.deltaTime * 2;
    private bool PlayerSeeOnTarget => Vector3.Angle(Vector3.ProjectOnPlane(playerCam.forward, transform.up), Vector3.ProjectOnPlane(playerPos.forward, transform.up)) < 2;

    void Start()
    {
        ToStart();
    }
    void Update()
    {
        if (move)
        {
            if (MoveToTarget() & RotateToAngle())
            {
                move = false;
                StartInput();
            }
        }
    }

    public void SetPlayer(GravFPS target)
    {
        gravFPS = target;
        player = gravFPS.transform;
        playerCam = player.GetChild(0);
        gravFPS.Status = PlayerState.disactive;
        move = true;
    }
    public void AddSymbol(int value)
    {
        currentCodeImages.Add(Instantiate(prefabs[value], pointsForInput[currentCode.Length]));
        if(value <10)
        {
            currentCode += value.ToString();
        }
        else
        {
            switch (value)
            {
                case 10:
                    currentCode += "G";
                    break;
                case 11:
                    currentCode += "Y";
                    break;
                case 12:
                    currentCode += "O";
                    break;
                case 13:
                    currentCode += "P";
                    break;
                case 14:
                    currentCode += "B";
                    break;
                case 15:
                    currentCode += "R";
                    break;
            }
        }
        CheckCode();
    }
    public void Escape()
    {
        gravFPS.Status = PlayerState.active;
        MyCursor.OpportunityToChange = true;
        MyCursor.LockState = CursorLockMode.Locked;
        MyCursor.Visible = false;
    }
    public void Clear()
    {
        for (int i = 0; i < currentCodeImages.Count; i++)
        {
            Destroy(currentCodeImages[i]);
        }
        currentCodeImages.Clear();
        currentCode = string.Empty;
    }

    public override void Use()
    {
        active = true;
        activeToggle.SetActive(true);
    }
    public override void ToStart()
    {
        currentCodeImages = new List<GameObject>();
        currentAttempt = attemptsCount;
        currentCode = string.Empty;
        source = GetComponent<AudioSource>();
        
        if(active)
        {
            activeToggle.SetActive(true);
        }
        else
        {
            activeToggle.SetActive(false);
        }
    }

    private void CheckCode()
    {
        switch (ValidCode())
        {
            case 1:
                Success();
                break;
            case -1:
                Error();
                break;
        }
    }
    private int ValidCode()
    {
        if (currentCode.Equals(code))
        {
            return 1;
        }
        else if (currentCode.Length >= code.ToString().Length)
        {
            return -1;
        }
        return 0;
    }
    private void Error()
    {
        if (currentAttempt > 0)
        {
            currentAttempt -= 1;
            Clear();
            if (currentAttempt == 0)
            {
                Escape();
                Destroy(triger);
                Destroy(this);
            }
        }
    }
    private void Success()
    {
        if(activeToggle)
        {
            Escape();
            UseAll();
            Destroy(triger);
            GiveCoins();
            StartMessage();
        }
    }
    private void UseAll()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
    private void GiveCoins() 
    {
        if(coinsCount > 0)
        {
            coinsCount--;
            gravFPS.gravFPSUI.AddCoin();
            Invoke("GiveCoins", Time.deltaTime);
        }
        else
        {
            Destroy(this);
        }
    }

    private bool MoveToTarget()
    {
        if (PlayerNearWithTarget)
        {
            player.position = playerPos.position;
            return true;
        }
        else
        {
            Vector3 dir = playerPos.position - player.position;
            player.position += dir * moveSpeed * Time.deltaTime;
        }
        return false;
    }
    private bool RotateToAngle()
    {
        if (PlayerSeeOnTarget)
        {
            player.forward = playerPos.forward;
            return true;
        }
        else
        {
            player.rotation = Quaternion.Lerp(player.rotation, playerPos.rotation, moveSpeed * Time.deltaTime);
        }
        return false;
    }

    private void StartInput()
    {
        MyCursor.LockState = CursorLockMode.None;
        MyCursor.Visible = true;
        MyCursor.OpportunityToChange = false;
    }
    private void StartMessage()
    {
        subsPanel.SetActive(true);
        subsText.text = messageText;
        source.PlayOneShot(messageClip);
        Invoke("StopMessage", messageClip.length + 0.5f);
    }
    private void StopMessage()
    {
        subsPanel.SetActive(false);
        
    }
}
