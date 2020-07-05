using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravChangeReactor : UsingOrigin
{
    public PlayerStateController playerStateController;

    private void Start()
    {
        if(playerStateController == null)
        {
            Debug.LogError("Не был передан компонент gravFPS в " + name);
        }
        else
        {
            playerStateController.playerGravMoveController.OnGravChange += Use;
        }
    }

    public override void ToStart()
    {
    }
    public override void Use()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
}
