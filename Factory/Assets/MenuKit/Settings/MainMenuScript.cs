using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("GameLoadData")]
[XmlInclude(typeof(LevelModuleStatus))]
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

        string datapath = Application.dataPath + "/Saves/SavedData/LoadData" + statusPack.loadSlot;
        System.Type[] extraTypes = { typeof(LevelModuleStatus), typeof(PosPack) };
        XmlSerializer serializer = new XmlSerializer(typeof(LoadData), extraTypes);

        FileStream fs = new FileStream(datapath, FileMode.OpenOrCreate);
        serializer.Serialize(fs, data);
        fs.Close();
    }
    public static LoadData LoadXML(int slot)
    {
        string datapath = Application.dataPath + "/Saves/SavedData/LoadData" + slot;
        System.Type[] extraTypes = { typeof(LevelModuleStatus), typeof(PosPack) };
        XmlSerializer serializer = new XmlSerializer(typeof(LoadData), extraTypes);

        FileStream fs = new FileStream(datapath, FileMode.Open);
        LoadData data = (LoadData)serializer.Deserialize(fs);
        fs.Close();

        return data;
    }
}

public class MainMenuScript : MonoBehaviour
{
    public List<GameObject> panels;
    public StatusPack playerStatusPack;

    private AsyncOperation loader;
    private bool inSubMenu;

    void Start()
    {
        loader = SceneManager.LoadSceneAsync("Load");
        for (int i = 0; i < panels.Count; i++)
        {
            ClosePanel(i);
        }
    }

    public void ShowPanel(int index)
    {
        if(!inSubMenu)
        {
            panels[index].SetActive(true);
        }
    }
    public void ClosePanel(int index)
    {
        panels[index].SetActive(false);
    }

    public void LoadScene()
    {

    }
}
