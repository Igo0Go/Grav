using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneManagementController : PlayerControllerBlueprint
{
    public PlayerStartSceneSettingsScript playerStartSceneSettings;
    public bool inHub = false;

    private AsyncOperation loader;


    private SaveLocationPack SaveLocationPack => PlayerStateController.playerGravMoveController.SaveLocationPack;
    private PlayerUIController PlayerUIController => PlayerStateController.playerUIController;
    private StatusPack StatusPack => PlayerStateController.statusPack;

    public event Action<Transform> SaveLocationWithPointEvent;
    public event Action<bool> VisualizeSaveEvent;
    public event Action CheckSavePlanetEvent;
    public event Action SaveLocationEvent;
    public event Action RemoveSphereEvent;
    public event Action OpportunityToLoadEvent;
    public event Action SetMaxHealthEvent;
    public event Action OnRestartEvent;

    protected override void SetReferences(PlayerStateController playerState)
    {
        playerState.playerReactionsController.DeathEvent += CheckLoad;


        StatusPack.currentScene = SceneManager.GetActiveScene().name;
        if (inHub)
        {
            StatusPack.hubScene = StatusPack.currentScene;
        }

        if (playerStartSceneSettings == null)
        {
            if (PlayerStateController.playerGravMoveController.planet != null)
            {
                PlayerStateController.playerGravMoveController.SetGravObj(PlayerStateController.playerGravMoveController.planet);
            }
            else
            {
                PlayerStateController.playerGravMoveController.SetGravVector(new Vector3(0, -9.81f, 0));
            }
        }
        else
        {
            playerStartSceneSettings.SetSettings(PlayerStateController);
        }
    }

    private void Start()
    {
        loader = SceneManager.LoadSceneAsync("Load");
        loader.allowSceneActivation = false;
    }

    /// <summary>
    /// Даёт команду на запуск предзагруженной сцены
    /// </summary>
    public void LoadNextScene()
    {
        loader.allowSceneActivation = true;
    }
    /// <summary>
    /// Перезапустить сцену с вычетом одной сферы (для случая, когда умер, а точки сохранения ещё не было)
    /// </summary>
    public void RestartSceneWithLoadSphere()
    {
        PlayerUIController.RemoveLifeSphere();
        RestartScene();
    }
    /// <summary>
    /// Загружает сцену хаба
    /// </summary>
    public void LoadHubScene()
    {
        PlayerUIController.CheckSpheres();
        ReturnStats();
        LoadNextScene();
    }
    /// <summary>
    /// Возвращает значения количества ресурсов к последним сохранённым
    /// </summary>
    public void ReturnStats()
    {
        StatusPack.currentMoneyCount = StatusPack.saveMoney;
        StatusPack.currentScene = StatusPack.hubScene;
        StatusPack.currentAcidCount = StatusPack.saveAcidCount;
    }
    /// <summary>
    /// Записывает точку для возрождения с учётм гравитации исходя из текущей позиции игрока
    /// </summary>
    public void Save()
    {
        SaveLocationEvent?.Invoke();
        VisualizeSaveEvent?.Invoke(false);
    }
    /// <summary>
    /// Записывает точку для возрождения с учётом гравитации исходя из заданной точки
    /// </summary>
    /// <param name="point"></param>
    public void Save(Transform point)
    {
        SaveLocationWithPointEvent?.Invoke(point);
        VisualizeSaveEvent?.Invoke(false);
    }


    private void RestartRun()
    {
        SetMaxHealthEvent?.Invoke();
        OnRestartEvent?.Invoke();
        VisualizeSaveEvent?.Invoke(false);
    }
    private void RestartScene()
    {
        StatusPack.currentMoneyCount = StatusPack.saveMoney;
        LoadNextScene();
    }
    private void ReturnToSavePoint()
    {
        RemoveSphereEvent?.Invoke();
        CheckSavePlanetEvent?.Invoke();
        //OnDeadEvent?.Invoke();         //?
        Invoke("RestartRun", 2);
        PlayerStateController.savePoint.OnRestart();
    }

    private void CheckLoad()
    {
        if (inHub)
        {
            if (PlayerStateController.savePoint != null)
            {
                if (StatusPack.currentLifeSphereCount > 0)
                {
                    ReturnToSavePoint();
                }
                else
                {
                    LoadHubScene();
                }
            }
            else
            {
                if (StatusPack.currentLifeSphereCount > 0)
                {
                    RemoveSphereEvent?.Invoke();
                }
                LoadHubScene();
            }
        }
        else
        {
            if (PlayerStateController.savePoint != null)
            {
                if (StatusPack.currentLifeSphereCount > 0)
                {
                    ReturnToSavePoint();
                }
                else
                {
                    LoadHubScene();
                }
            }
            else
            {
                if (StatusPack.currentLifeSphereCount > 0)
                {
                    OpportunityToLoadEvent?.Invoke();
                }
                else
                {
                    LoadHubScene();
                }
            }
        }
    }
}
