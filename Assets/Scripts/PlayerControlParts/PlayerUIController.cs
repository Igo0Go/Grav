using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditorInternal;

public class PlayerUIController : PlayerControllerBlueprint
{
    #region Публичные поля (UI)
    public Slider healthSlider;
    public GameObject deadPanel;
    public List<StatisticksPanel> panels;
    public GameObject loadPanel;
    public Text tip;
    public Image stunPanel;
    [Range(0.1f, 2)] public float stunSpeed;
    #endregion

    #region Приватные поля
    private InputSettingsManager manager;
    private float returnTime;
    private float targetHelathSlideValue;
    private bool spendMoney;
    private bool changeHealth;
    #endregion

    #region Свойства
    public float HealthSliderValue
    {
        get
        {
            return healthSlider.value;
        }
        set
        {
            healthSlider.value = value;
        }
    }
    public bool InMenu { get; set; }
    public StatusPack StatusPack => PlayerStateController.statusPack;
    #endregion

    #region Делегаты и события
    public event Action OnGetLoot;
    public event Action OnFinalStun;
    #endregion

    #region Основное
    protected override void SetReferences(PlayerStateController playerState)
    {
        playerState.playerInputController.StatsInputEvent += StatisticInput;
        playerState.playerReactionsController.HealthChanged += SetTargetForHealthValueSlider;
        playerState.playerReactionsController.VisualizeDeathEvent += SetDeadPanelState;
        playerState.playerSceneManagementController.VisualizeSaveEvent += SetDeadPanelState;
        playerState.playerSceneManagementController.RemoveSphereEvent += RemoveLifeSphere;
        playerState.playerSceneManagementController.OpportunityToLoadEvent += OpenPanelForReload;
        playerState.gravityThrower.AcidShootEvent += OnAcidShoot;
        playerState.playerReactionsController.ActionTipMessageActivated += SetTip;
        OnFinalStun += playerState.ReturnActive;

        InMenu = false;
        manager = playerState.playerInputController.inputSettingsManager;
        loadPanel.SetActive(false);
        healthSlider.maxValue = HealthSliderValue = 100;
        panels[0].text.text = StatusPack.currentMoneyCount.ToString();
        panels[1].text.text = StatusPack.currentLifeSphereCount.ToString();
        panels[2].text.text = StatusPack.currentAcidCount.ToString();
        foreach (var item in panels)
        {
            item.anim.SetBool("Visible", true);
        }

        returnTime = 3;

        StatusPack.saveSphereCount = StatusPack.currentLifeSphereCount;
    }

    void Update()
    {
        ChangeHealthSliderValueSmoth();
        ChangeReturnTime();
        SpendMoney();
        StatisticInput();
        ChangeStunPanel();
    }
    #endregion

    #region Публичные методы
    /// <summary>
    /// Актуализирует статистику - задаёт конкретное количество ресурсов в текстах статистики
    /// </summary>
    public void CheckTexts()
    {
        panels[0].text.text = StatusPack.currentMoneyCount.ToString();
        panels[1].text.text = StatusPack.currentLifeSphereCount.ToString();
        panels[2].text.text = StatusPack.currentAcidCount.ToString();
    }
    /// <summary>
    /// Обновляет статистику - добавляет монетку
    /// </summary>
    public void AddCoin()
    {
        StatusPack.currentMoneyCount++;
        CheckTexts();
        panels[0].anim.SetBool("Visible", true);
        returnTime = 3;
    }
    /// <summary>
    /// Обновляет статистику - повышает уровень кислоты на значение value (с учётом максимального значения)
    /// </summary>
    /// <param name="value"></param>
    public void AddAcid(float value)
    {
        StatusPack.currentAcidCount += value;
        StatusPack.currentAcidCount = Mathf.Clamp(StatusPack.currentAcidCount, 0, StatusPack.maxAcidCount);
        CheckTexts();
        panels[2].anim.SetBool("Visible", true);
        returnTime = 3;
        OnGetLoot?.Invoke();
    }
    /// <summary>
    /// Обновляет статистику - добавляет сферу клонирования
    /// </summary>
    public void AddLifeSphere()
    {
        StatusPack.currentLifeSphereCount++;
        CheckTexts();
        panels[1].anim.SetBool("Visible", true);
        returnTime = 3;
    }
    /// <summary>
    /// Обновляет статистику - добавляет банковскую карточку с указанным индексом
    /// </summary>
    /// <param name="number"></param>
    public void AddBankCard(int number)
    {
        StatusPack.cards[number] = true;
    }
    /// <summary>
    /// Обновляет статистику - удаляет карточку с указанным индексом
    /// </summary>
    /// <param name="number"></param>
    public void RemoveCard(int number)
    {
        StatusPack.cards[number] = false;
    }
    /// <summary>
    /// Обновляет статистику - удаляет одну сферу клонирования
    /// </summary>
    public void RemoveLifeSphere()
    {
        StatusPack.currentLifeSphereCount--;
        CheckTexts();
        panels[1].anim.SetBool("Visible", true);
        returnTime = 3;
    }
    /// <summary>
    /// При возвращении в хаб, при условии, что миссия не пройдена, требуется установить, насколько много жизней потерял игрок. Если он собрал жизни и решил выйти,
    /// этот метод откатит количество жизней к значению до миссии, а если во время миссии он потерял часть жизнией, то ему сохранят только оставшиеся. Данный метод
    /// не позволяет игроку нечестным путём добавлять себе большое количество жизней путём входов в миссию, сбора сфер на ней, и выхода.
    /// </summary>
    public void CheckSpheres()
    {
        StatusPack.currentLifeSphereCount = StatusPack.saveSphereCount > StatusPack.currentLifeSphereCount ? StatusPack.currentLifeSphereCount : StatusPack.saveSphereCount;
    }
    /// <summary>
    /// Сохраняет статистику - количество собранных ресурсов
    /// </summary>
    public void SaveStats()
    {
        StatusPack.saveMoney = StatusPack.currentMoneyCount;
        StatusPack.saveAcidCount = StatusPack.currentAcidCount;
        StatusPack.saveSphereCount = StatusPack.currentLifeSphereCount;
        StatusPack.saveCards = new List<bool>();
        foreach (var item in StatusPack.cards)
        {
            StatusPack.saveCards.Add(item);
        }
    }
    /// <summary>
    /// Уводит все показатели статистики с экрана
    /// </summary>
    public void ReturnAllStatsPanel()
    {
        foreach (var item in panels)
        {
            item.anim.SetBool("Visible", false);
        }
    }
    /// <summary>
    /// Выводит текстовую подсказку о возможном действии с учётом кнопки
    /// </summary>
    /// <param name="lootPoint"></param>
    /// <param name="manager"></param>
    public void SetTip(string tipText, bool opportunityToAction)
    {
        if (opportunityToAction)
            tip.text = manager.GetKey("Using").ToString() + " - " + tipText;
        else
            tip.text = tipText;
    }
    /// <summary>
    /// Очищает текст подсказки
    /// </summary>
    public void ClearTip()
    {
        tip.text = string.Empty;
    }
    /// <summary>
    /// Потратить указанное количество монет
    /// </summary>
    /// <param name="count"></param>
    public void Spend(int count)
    {
        if (!spendMoney)
        {
            StatusPack.currentMoneyCount -= count;
            spendMoney = true;
        }
    }
    /// <summary>
    /// Активирует панель при оглушении
    /// </summary>
    public void SetStunPanel()
    {
        stunPanel.gameObject.SetActive(true);
        stunPanel.color = new Color(stunPanel.color.r, stunPanel.color.g, stunPanel.color.b, 1);
    }
    #endregion

    #region Служебные методы
    private void SetTargetForHealthValueSlider(float target)
    {
        targetHelathSlideValue = target;
        changeHealth = true;
    }
    private void StatisticInput()
    {
        foreach (var item in panels)
        {
            item.anim.SetBool("Visible", true);
        }
        returnTime = 3;
    }

    private void ReturnSpend() => spendMoney = true;
    private void SpendMoney()
    {
        if (spendMoney)
        {
            int currentCoinCountInText = int.Parse(panels[0].text.text);
            if (currentCoinCountInText > StatusPack.currentMoneyCount)
            {
                currentCoinCountInText--;
                panels[0].text.text = currentCoinCountInText.ToString();
                panels[0].anim.SetBool("Visible", true);
                returnTime = 1;
                Invoke("ReturnSpend", 0.1f);
            }
            spendMoney = false;
        }
    }

    private void OnAcidShoot()
    {
        StatusPack.currentAcidCount--;
        StatusPack.currentAcidCount = Mathf.Clamp(StatusPack.currentAcidCount, 0, StatusPack.maxAcidCount);
        CheckTexts();
    }
    private void ChangeStunPanel()
    {
        if (stunPanel.gameObject.activeSelf)
        {
            if (stunPanel.color.a - Time.deltaTime * stunSpeed > 0)
            {
                stunPanel.color = new Color(stunPanel.color.r, stunPanel.color.g, stunPanel.color.b, stunPanel.color.a - Time.deltaTime * stunSpeed);
            }
            else
            {
                stunPanel.color = new Color(stunPanel.color.r, stunPanel.color.g, stunPanel.color.b, 0);
                stunPanel.gameObject.SetActive(false);
                OnFinalStun?.Invoke();
            }
        }
    }
    private void ChangeHealthSliderValueSmoth()
    {
        if(changeHealth)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHelathSlideValue, Time.deltaTime * 10);
            if(Mathf.Abs(healthSlider.value - targetHelathSlideValue) < 3)
            {
                healthSlider.value = targetHelathSlideValue;
                changeHealth = false;
            }
        }
    }
    private void ChangeReturnTime()
    {
        returnTime -= Time.deltaTime;
        if (returnTime <= 0)
        {
            returnTime = 0;
            ReturnAllStatsPanel();
        }
    }

    private void SetDeadPanelState(bool value)
    {
        deadPanel.SetActive(value);
    }
    private void OpenPanelForReload()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        loadPanel.SetActive(true);
        panels[1].anim.SetBool("Visible", true);
    }
    #endregion
}

[Serializable]
public class StatisticksPanel
{
    public Text text;
    public Animator anim;
}
