using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataLoaderController : PlayerControllerBlueprint
{
    private StatusPack StatusPack => PlayerStateController.statusPack;

    protected override void SetReferences(PlayerStateController playerState)
    {

    }

    private void SetInput()
    {
        if (DataLoader.LoadXML(StatusPack.loadSlot, out LoadData data))
        {
            PlayerStateController.playerInputController.inputSettingsManager.CopySettings(data.inputKit);
        }
    }
    private void SaveInput()
    {
        DataLoader.SaveXML(StatusPack, PlayerStateController.playerInputController.inputSettingsManager.inputKit);
    }
}
