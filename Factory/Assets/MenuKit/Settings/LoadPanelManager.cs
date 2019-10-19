using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPanelManager : MonoBehaviour
{
    public List<LoadSlot> slots;

    private void OnEnable()
    {
        CheckLoad();
    }
    private void CheckLoad()
    {
        LoadData data;
        for (int i = 1; i < 4; i++)
        {
            if (DataLoader.LoadXML(i, out data))
            {
                slots[i-1].gameObject.SetActive(true);
                slots[i-1].DrawData(data);
            }
            else
            {
                slots[i-1].gameObject.SetActive(false);
            }
        }
    }
}
