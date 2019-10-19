using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("GameLoadData")]
public class LoadData
{
    public StatusPack statusPack;

    [XmlArray("LevelModuleStatusKit")]
    [XmlArrayItem("LevelModuleStatusItem")]
    public List<LevelModuleStatus> levelModuleStatusKit;

    public LoadData() { }
}

public static class DataLoader
{
    public static void SaveXML(StatusPack statusPack)
    {
        LoadData data = new LoadData();
        data.statusPack = statusPack;
        data.levelModuleStatusKit = LevelModuleStatusSettings.levelModuleStatusList;

        
        System.Type[] extraTypes = { typeof(LevelModuleStatus), typeof(PosPack) };
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
        System.Type[] extraTypes = { typeof(LevelModuleStatus), typeof(PosPack) };
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
        DataLoader.SaveXML(playerStatusPack);
        loader.allowSceneActivation = true;
    }
    public void Exit()
    {
        Application.Quit();
    }
}
