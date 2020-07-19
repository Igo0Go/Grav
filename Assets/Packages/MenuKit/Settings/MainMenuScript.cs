using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            
            var inputPanel = panels[i].GetComponent<InputSettingsMenuScript>();
            if (inputPanel)
            {
                inputPanel.ReturnEvent += ReturnToMainPanel;
                inputPanel.Initialize();
            }
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
        DataLoader.RemoveXML(slot);
        playerStatusPack.loadSlot = slot;
        playerStatusPack.saveAcidCount = playerStatusPack.saveSphereCount = playerStatusPack.saveMoney =
            playerStatusPack.currentLifeSphereCount = playerStatusPack.currentMoneyCount = 0;
        playerStatusPack.currentAcidCount = 0;
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

    private void ReturnToMainPanel()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            ClosePanel(i);
        }
    }
}
