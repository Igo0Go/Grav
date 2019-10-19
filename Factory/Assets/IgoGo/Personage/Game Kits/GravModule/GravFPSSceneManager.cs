using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GravFPSSceneManager : MonoBehaviour
{
    private AsyncOperation loader;

    [HideInInspector] public StatusPack pack;
    void Start()
    {
        loader = SceneManager.LoadSceneAsync("Load");
        loader.allowSceneActivation = false;
    }

    public void LoadNextScene()
    {
        loader.allowSceneActivation = true;
    }
}
