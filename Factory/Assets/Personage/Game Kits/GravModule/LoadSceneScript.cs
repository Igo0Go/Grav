using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneScript : UsingObject
{
    public StatusPack pack;
    public int loadType;
    public string sceneName;

    public AsyncOperation loader;
    private void Start()
    {
        if(loadType == 0)
        {
            loader = SceneManager.LoadSceneAsync(pack.currentScene);
            loader.allowSceneActivation = true;
        }
        else if (loadType == 1)
        {
            loader = SceneManager.LoadSceneAsync(pack.hubScene);
            loader.allowSceneActivation = true;
        }
    }

    public override void Use()
    {
        if(loadType==2)
        {
            pack.currentScene = sceneName;
        }
        else
        {
            pack.currentScene = pack.hubScene = sceneName;
        }
    }
}
