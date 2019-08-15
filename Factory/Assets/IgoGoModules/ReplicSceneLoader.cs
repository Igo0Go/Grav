using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplicSceneLoader : UsingObject
{
    [SerializeField]
    [Tooltip("Название загружаемой сцены")] private string sceneName = "Menu";
    [SerializeField]
    [Tooltip("Не нужно дополнительное действие")] private bool trigger = false;
    public AsyncOperation loader;
    private bool replicComplete;

    private void Start()
    {
        LoadManager.NameSceneForLoad = sceneName;
        loader = SceneManager.LoadSceneAsync("Load");
        loader.allowSceneActivation = false;
    }
    public void CompleteReplic()
    {
        replicComplete = true;
        CheckComplete();
    }
    private void CheckComplete()
    {
        if (replicComplete && trigger)
        {
            LoadNextScene();
        }
    }
    private void LoadNextScene()
    {
        loader.allowSceneActivation = true;
    }
    public override void Use()
    {
        trigger = true;
        CheckComplete();
    }
}