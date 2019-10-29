using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStartSceneSettingsItem
{
    public Transform point;
    public SphereGravModule gravObj;
    public Vector3 gravVector;
}

public class PlayerStartSceneSettingsScript : MonoBehaviour
{
    public List<PlayerStartSceneSettingsItem> items;

    public void SetSettings(GravFPS player)
    {
        if (player.gravFPSUI.StatusPack.hubPoint > items.Count - 1)
        {
            Debug.LogError("Нет позиции для игрока с указанным индексом.");
        }
        else
        {
            PlayerStartSceneSettingsItem item = items[player.gravFPSUI.StatusPack.hubPoint];
            player.transform.position = item.point.position;
            player.transform.rotation = item.point.rotation;
            if (item.gravObj != null)
            {
                player.SetGravObj(item.gravObj);
            }
            else
            {
                player.SetGravVector(item.gravVector);
            }
        }
    }
}
