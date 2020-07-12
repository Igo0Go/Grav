using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : PlayerControllerBlueprint
{
    public Animator gunAnimator;

    protected override void SetReferences(PlayerStateController playerState)
    {

    }

    public void SetReadyAnimation(bool value)
    {
        gunAnimator.SetBool("Ready", value);
    }
}
