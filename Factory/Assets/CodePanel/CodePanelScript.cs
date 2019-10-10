using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodePanelScript : UsingOrigin
{
    [SerializeField] private Transform playerPos;
    [SerializeField] private Collider triger;
    [SerializeField, Range(1, 3)] float moveSpeed; 

    [SerializeField, Space(10)] private Text codeText;
    [SerializeField] private Text attemptsText;
    [SerializeField] private Text messageText;

    [SerializeField, Space(10), Range(1,10)] private int attemptsCount;
    [SerializeField] private int code;
    [SerializeField] private string message;
    [SerializeField] private string blockText;

    private GravFPS gravFPS;
    private Transform player;
    private Transform playerCam;
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
        if(move)
        {
            if(MoveToTarget() & RotateToAngle())
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
        gravFPS.status = PlayerState.disactive;
        move = true;
    }
    public void AddSymbol(int value)
    {
        codeText.text += value.ToString();
        CheckCode();
    }
    public void Escape()
    {
        gravFPS.status = PlayerState.active;
        MyCursor.OpportunityToChange = true;
        MyCursor.LockState = CursorLockMode.Locked;
        MyCursor.Visible = false;
    }
    public void Clear()
    {
        codeText.text = string.Empty;
    }

    public override void Use()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
    public override void ToStart()
    {
        codeText.text = string.Empty;
        messageText.text = string.Empty;
        currentAttempt = attemptsCount;
        attemptsText.text = "Попыток: " + currentAttempt;
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
        int codeTextValue = 0;
        int codeSymbolsCount = code.ToString().Length;
        if(int.TryParse(codeText.text, out codeTextValue))
        {
            if(codeTextValue == code)
            {
                return 1;
            }
            else if(codeText.text.Length >= code.ToString().Length)
            {
                return -1;
            }
        }
        return 0;
    }
    private void Error()
    {
        if(currentAttempt > 0)
        {
            currentAttempt -= 1;
            attemptsText.text = "Попыток: " + currentAttempt;
            codeText.text = string.Empty;
            if(currentAttempt == 0)
            {
                Escape();
                Destroy(triger);
                messageText.text = blockText;
                Destroy(this);
            }
        }
    }
    private void Success()
    {
        Escape();
        Use();
        Destroy(triger);
        messageText.text = message;
        Destroy(this);
    }

    private bool MoveToTarget()
    {
        if(PlayerNearWithTarget)
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
        if(PlayerSeeOnTarget)
        {
            player.forward = playerPos.forward;
            return true;
        }
        else
        {
            player.rotation = Quaternion.Lerp(player.rotation, playerPos.rotation, moveSpeed*Time.deltaTime);
        }
        return false;
    }

    private void StartInput()
    {
        MyCursor.LockState = CursorLockMode.None;
        MyCursor.Visible = true;
        MyCursor.OpportunityToChange = false;
    }
}
