using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : MonoBehaviour
{
    public int slotIndex;
    public Text title;
    public Text lifeSphereCount;
    public Text coinsCount;
    public Text acidCount;
    public List<GameObject> cardToggles;
    public MainMenuScript mainMenu;

    public void DrawData(LoadData data)
    {
        lifeSphereCount.text = data.statusPack.saveSphere.ToString();
        coinsCount.text = data.statusPack.saveMoney.ToString();
        acidCount.text = data.statusPack.saveAcidCount.ToString() + "/" + data.statusPack.maxAcidCount.ToString();

        for (int i = 0; i < data.statusPack.saveCards.Count; i++)
        {
            cardToggles[i].SetActive(data.statusPack.saveCards[i]);
        }
        title.text = data.statusPack.hubScene;
    }
    public void Load()
    {
        if(DataLoader.LoadXML(slotIndex, out LoadData data))
        {
            CopyStatus(mainMenu.playerStatusPack,data.statusPack);
            LevelModuleStatusSettings.levelModuleStatusList = data.levelModuleStatusKit;
            mainMenu.LoadScene();
        }
    }
    public void Delete()
    {
        DataLoader.RemoveXML(slotIndex);
        gameObject.SetActive(false);
    }

    private void CopyStatus(StatusPack left, StatusPack right)
    {
        left.saveAcidCount = right.saveAcidCount;
        left.saveMoney = right.saveMoney;
        left.saveSphere = right.saveSphere;

        left.money = right.money;
        left.lifeSphereCount = right.lifeSphereCount;
        left.acidCount = right.acidCount;
        left.maxAcidCount = right.maxAcidCount;

        left.loadSlot = right.loadSlot;
        left.currentScene = right.currentScene;
        left.hubScene = right.hubScene;
        left.hubPoint = right.hubPoint;

        left.cards = new List<bool>();
        left.saveCards = new List<bool>();

        foreach (var item in right.cards)
        {
            left.cards.Add(item);
        }
        foreach (var item in right.saveCards)
        {
            left.saveCards.Add(item);
        }
    }
}
