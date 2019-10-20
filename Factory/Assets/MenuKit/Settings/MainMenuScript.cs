using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

[XmlType("StatusPcakContainer")]
public class StatusPackContainer
{
    public int loadSlot;
    public string currentScene;
    public string hubScene;

    public int loadStatus;

    public int money;
    public int lifeSphereCount;
    public float acidCount;

    public int saveMoney;
    public int saveSphere;
    public float saveAcidCount;
    public float maxAcidCount;

    public int hubPoint;

    public List<bool> cards;
    public List<bool> saveCards;

    public StatusPackContainer() { }
    public StatusPackContainer(StatusPack pack)
    {
        loadSlot = pack.loadSlot;
        currentScene = pack.currentScene;
        hubScene = pack.hubScene;
        loadStatus = pack.loadStatus;
        money = pack.money;
        lifeSphereCount = pack.lifeSphereCount;
        acidCount = pack.acidCount;
        saveMoney = pack.saveMoney;
        saveSphere = pack.saveSphere;
        saveAcidCount = pack.saveAcidCount;
        maxAcidCount = pack.maxAcidCount;
        hubPoint = pack.hubPoint;

        cards = new List<bool>();
        foreach (var item in pack.cards)
        {
            cards.Add(item);
        }
        saveCards = new List<bool>();
        foreach (var item in pack.saveCards)
        {
            saveCards.Add(item);
        }
    }
}

[XmlType("AudioSettingsPackContainer")]
public class AudioSettingsPackContainer
{
    [Tooltip("множитель для громкости музыки"), Range(0, 1)] public float musicMultiplicator;
    [Tooltip("множитель для громкости музыки"), Range(0, 1)] public float otherAudioMultiplicator;

    public AudioSettingsPackContainer() { }
    public AudioSettingsPackContainer(bool useSettings)
    {
        musicMultiplicator = AudioSettingsPack.musicMultiplicator;
        otherAudioMultiplicator = AudioSettingsPack.otherAudioMultiplicator;
    }
}

[XmlRoot("GameLoadData")]
public class LoadData
{
    public StatusPackContainer statusPack;
    public AudioSettingsPackContainer audioSettings;

    [XmlArray("LevelModuleStatusKit")]
    [XmlArrayItem("LevelModuleStatusItem")]
    public List<LevelModuleStatus> levelModuleStatusKit;
    public InputKit inputKit;

    public LoadData() { }
}

public static class DataLoader
{
    public static void SaveXML(StatusPack statusPack, InputKit inputKit)
    {
        LoadData data = new LoadData();
        data.statusPack = new StatusPackContainer(statusPack);
        data.levelModuleStatusKit = LevelModuleStatusSettings.levelModuleStatusList;
        data.audioSettings = new AudioSettingsPackContainer(true);
        data.inputKit = inputKit;

        System.Type[] extraTypes = { typeof(StatusPackContainer), typeof(LevelModuleStatus), typeof(PosPack), typeof(KeyCodeContainer),
            typeof(KeyCode), typeof(AxisContainer), typeof(AudioSettingsPackContainer)};
        XmlSerializer serializer = new XmlSerializer(typeof(LoadData), extraTypes);

        string datapath = Application.dataPath + "/Saves";

        if (!Directory.Exists(datapath))
        {
            Directory.CreateDirectory(datapath);
        }
        datapath += "/Slot" + statusPack.loadSlot;

        FileStream fs = new FileStream(datapath, FileMode.OpenOrCreate);
        serializer.Serialize(fs, data);
        fs.Close();
    }
    public static bool LoadXML(int slot, out LoadData data)
    {
        data = null;
        System.Type[] extraTypes = { typeof(StatusPackContainer), typeof(LevelModuleStatus), typeof(PosPack), typeof(KeyCodeContainer),
            typeof(KeyCode), typeof(AxisContainer), typeof(AudioSettingsPackContainer)};
        XmlSerializer serializer = new XmlSerializer(typeof(LoadData), extraTypes);
        string datapath = Application.dataPath + "/Saves";
        if (!Directory.Exists(datapath))
        {
            return false;
        }
        datapath += "/Slot" + slot;
        if(!File.Exists(datapath))
        {
            return false;
        }
        FileStream fs = new FileStream(datapath, FileMode.Open);
        data = (LoadData)serializer.Deserialize(fs);
        fs.Close();
        return true;
    }
    public static void RemoveXML(int slot)
    {
        string datapath = Application.dataPath + "/Saves";
        if (Directory.Exists(datapath))
        {
            datapath += "/Slot" + slot;
            if (File.Exists(datapath))
            {
                File.Delete(datapath);
            }
        }
    }
}

public class MainMenuScript : MonoBehaviour
{
    public List<GameObject> panels;
    public GameObject mainMenuPanel;
    public StatusPack playerStatusPack;
    public InputKit inputKit;
    public string startScene;

    private AsyncOperation loader;

    void Start()
    {
        loader = SceneManager.LoadSceneAsync("Load");
        loader.allowSceneActivation = false;
        for (int i = 0; i < panels.Count; i++)
        {
            ClosePanel(i);
        }
        mainMenuPanel.SetActive(true);
    }

    public void ShowPanel(int index)
    {
        panels[index].SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    public void ClosePanel(int index)
    {
        panels[index].SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void LoadScene()
    {
        loader.allowSceneActivation = true;
    }
    public void NewGame(int slot)
    {
        playerStatusPack.loadSlot = slot;
        playerStatusPack.saveAcidCount = playerStatusPack.saveSphere = playerStatusPack.saveMoney = playerStatusPack.lifeSphereCount = playerStatusPack.money = 0;
        playerStatusPack.acidCount = 0;
        playerStatusPack.maxAcidCount = 5;
        playerStatusPack.saveCards = playerStatusPack.cards = new List<bool>() { false, false, false, false };
        playerStatusPack.currentScene = playerStatusPack.hubScene = startScene;
        playerStatusPack.hubPoint = 0;
        LevelModuleStatusSettings.levelModuleStatusList.Clear();
        DataLoader.SaveXML(playerStatusPack, inputKit);
        loader.allowSceneActivation = true;
    }
    public void Exit()
    {
        Application.Quit();
    }
}
