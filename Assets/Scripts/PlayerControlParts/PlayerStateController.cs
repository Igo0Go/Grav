using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PlayerAudioController))]
[RequireComponent(typeof(PlayerCameraController))]
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(PlayerGravMoveController))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerUIController))]
[RequireComponent(typeof(PlayerReactionsController))]
[RequireComponent(typeof(PlayerSceneManagementController))]
public class PlayerStateController : MonoBehaviour
{
    public PlayerInputController playerInputController;
    public PlayerGravMoveController playerGravMoveController;
    public PlayerAnimationController playerAnimationController;
    public PlayerUIController playerUIController;
    public PlayerReactionsController playerReactionsController;
    public PlayerSceneManagementController playerSceneManagementController;
    public GravityThrowerScript gravityThrower;
    public PlayerCameraController playerCameraController;
    public PlayerAudioController playerAudioController;

    [Space(10)]public StatusPack statusPack;

    public event Action<bool> ReadyEvent;

    public SavePoint savePoint { get; set; }
    public PlayerState Status
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            if (_state <= 0)
            {
                ReadyEvent?.Invoke(false);
            }
            else
            {
                ReadyEvent?.Invoke(true);
            }
        }
    }
    private PlayerState _state;
    public bool InHub => playerSceneManagementController.inHub;
    public bool InMenu
    {
        get { return playerUIController.InMenu; }
        set { playerUIController.InMenu = value; }
    }
    public bool Alive => playerReactionsController.Alive;

    private void Awake()
    {
        Status = PlayerState.active;
        
        playerInputController.InitController(this);
        playerGravMoveController.InitController(this);
        playerAnimationController.InitController(this);
        playerReactionsController.InitController(this);
        playerSceneManagementController.InitController(this);
        gravityThrower.InitController(this);
        playerCameraController.InitController(this);
        playerUIController.InitController(this);
        playerAudioController.InitController(this);

        SetCursorVisible(false);
    }
    public void SetCursorVisible(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void Stun()
    {
        playerUIController.SetStunPanel();
        Status = PlayerState.disactive;
    }
    public void ReturnActive() => Status = PlayerState.active;
}

public abstract class PlayerControllerBlueprint : MonoBehaviour
{
    protected Transform Mytransform => _myTransform;
    private Transform _myTransform;
    public PlayerStateController PlayerStateController => _playerStateController;
    private PlayerStateController _playerStateController;
    
    public void InitController(PlayerStateController playerState)
    {
        SetMainController(playerState);
        SetReferences(playerState);
    }

    private void SetMainController(PlayerStateController playerState)
    {
        _playerStateController = playerState;
        _myTransform = playerState.transform;
    }
    protected abstract void SetReferences(PlayerStateController playerState);
}
