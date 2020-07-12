using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public enum LoadType
{
    loadProcessing,
    loadMission,
    loadHub
}

public class LoadSceneScript : UsingObject
{
    public PlayerStateController playerStateController;
    public StatusPack pack;
    public ModuleController moduleConroller;
    public LoadType loadType;
    public string sceneName;
    public bool setHubPos;
    public int hubPos;

    public AsyncOperation loader;
    private void Start()
    {
        if(loadType == LoadType.loadProcessing)
        {
            loader = SceneManager.LoadSceneAsync(pack.currentScene);
            loader.allowSceneActivation = true;
        }
    }

    public override void Use()
    {
        if (setHubPos)
        {
            pack.hubPoint = hubPos;
        }
        used = true;
        if(moduleConroller != null)
        {
            moduleConroller.Save();
        }
        
        if(loadType==LoadType.loadMission)
        {
            pack.currentScene = sceneName;
        }
        else if(loadType == LoadType.loadHub)
        {
            pack.currentScene = pack.hubScene = sceneName;
        }
        playerStateController.playerUIController.SaveStats();
        playerStateController.playerSceneManagementController.LoadNextScene();
    }
    public override void ToStart()
    {
        used = false;
    }
}
