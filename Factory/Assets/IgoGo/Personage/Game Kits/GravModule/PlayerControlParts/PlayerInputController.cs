﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : PlayerControllerBlueprint
{
    public InputSettingsManager inputSettingsManager;

    /// <summary>
    /// method(horizontal, vertiacal)
    /// </summary>
    public event Action<float, float> MoveInputEvent;
    /// <summary>
    /// method(X, Y)
    /// </summary>
    public event Action<float, float> RotateInputEvent;
    public event Action<bool> SprintBottonDownEvent;
    public event Action JumpInputEvent;
    public event Action ShootInputEvent;
    public event Action ChangeGunTypeInputEvent;
    public event Action UsingInputEvent;
    public event Action DronInputEvent;
    public event Action ShieldInputEvent;
    public event Action StatsInputEvent;
    public event Action PauseInputEvent;

    protected override void SetReferences(PlayerStateController playerState)
    {

    }

    private void Update()
    {
        SprintInput();
        JumpInput();
        MoveInput();

        ShootInput();
        ChangeGunTypeInput();

        ShieldInput();
        DronInput();
        StatsInput();
        PauseInput();
        UsingInput();
    }
    private void LateUpdate()
    {
        RotateInput();
    }

    private void MoveInput()
    {
        MoveInputEvent?.Invoke(inputSettingsManager.GetAxis("Horizontal"), inputSettingsManager.GetAxis("Vertical"));
    }
    private void RotateInput()
    {
        RotateInputEvent?.Invoke(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
    private void SprintInput()
    {
        SprintBottonDownEvent?.Invoke(Input.GetKey(inputSettingsManager.GetKey("Sprint")));
    }
    private void JumpInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Jump")))
            JumpInputEvent?.Invoke();
    }
    private void ShootInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Fire1")))
            ShootInputEvent?.Invoke();
    }
    private void ChangeGunTypeInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Fire2")))
            ChangeGunTypeInputEvent?.Invoke();
    }
    private void ShieldInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Shield")))
            ShieldInputEvent?.Invoke();
    }
    private void DronInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Dron")))
        {
            DronInputEvent?.Invoke();
        }
    }
    private void StatsInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Stats")))
        {
            StatsInputEvent?.Invoke();
        }
    }
    private void PauseInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Pause")))
        {
            PauseInputEvent?.Invoke();
        }
    }
    private void UsingInput()
    {
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Using")))
        {
            UsingInputEvent?.Invoke();
        }
    }
}
