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
            CopyStatus(data.statusPack);
            CopySettings(data.audioSettings);
            CopySettings(data.inputKit);

            LevelModuleStatusSettings.levelModuleStatusList = data.levelModuleStatusKit;
            mainMenu.LoadScene();
        }
    }
    public void Delete()
    {
        DataLoader.RemoveXML(slotIndex);
        gameObject.SetActive(false);
    }

    private void CopyStatus(StatusPackContainer right)
    {
        mainMenu.playerStatusPack.saveAcidCount = right.saveAcidCount;
        mainMenu.playerStatusPack.saveMoney = right.saveMoney;
        mainMenu.playerStatusPack.saveSphereCount = right.saveSphere;

        mainMenu.playerStatusPack.currentMoneyCount = right.money;
        mainMenu.playerStatusPack.currentLifeSphereCount = right.lifeSphereCount;
        mainMenu.playerStatusPack.currentAcidCount = right.acidCount;
        mainMenu.playerStatusPack.maxAcidCount = right.maxAcidCount;

        mainMenu.playerStatusPack.loadSlot = right.loadSlot;
        mainMenu.playerStatusPack.currentScene = right.currentScene;
        mainMenu.playerStatusPack.hubScene = right.hubScene;
        mainMenu.playerStatusPack.hubPoint = right.hubPoint;

        mainMenu.playerStatusPack.cards = new List<bool>();
        mainMenu.playerStatusPack.saveCards = new List<bool>();

        foreach (var item in right.cards)
        {
            mainMenu.playerStatusPack.cards.Add(item);
        }
        foreach (var item in right.saveCards)
        {
            mainMenu.playerStatusPack.saveCards.Add(item);
        }
    }
    private void CopySettings(InputKitContainer right)
    {
        mainMenu.inputKit.sensivityMultiplicator = right.sensivityMultiplicator;

        mainMenu.inputKit.keys.Clear();
        for (int i = 0; i < right.keys.Count; i++)
        {
            mainMenu.inputKit.keys.Add(new KeyCodeContainer(right.keys[i]));
        }

        mainMenu.inputKit.axis.Clear();
        for (int i = 0; i < right.axis.Count; i++)
        {
            mainMenu.inputKit.axis.Add(new AxisContainer(right.axis[i]));
        }
    }
    private void CopySettings(AudioSettingsPackContainer right)
    {
        AudioSettingsPack.musicMultiplicator = right.musicMultiplicator;
        AudioSettingsPack.otherAudioMultiplicator = right.otherAudioMultiplicator;
    }
}
