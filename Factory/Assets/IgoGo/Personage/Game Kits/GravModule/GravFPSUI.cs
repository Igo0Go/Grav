using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class StatisticksPanel
{
    public Text text;
    public Animator anim;
}

public class GravFPSUI : MonoBehaviour
{
    #region Публичные поля (UI)
    [Header("UI")]
    public Slider healtSlider;
    public GameObject deadPanel;
    public List<StatisticksPanel> panels;
    public StatusPack StatusPack;
    public GameObject loadPanel;
    public Text tip;
    [HideInInspector] public InputSettingsManager manager;
    [HideInInspector] public float returnTime;
    #endregion

    #region Приватные поля
    private bool spendMoney;
    #endregion

    #region Свойства
    public float Health { get { return _health; } set { _health = value; } }
    private float _health;
    #endregion

    #region Делегаты и события
    public event Action onGetLoot;
    #endregion

    #region События Unity
    void Start()
    {
        loadPanel.SetActive(false);
        _health = 100;
        healtSlider.value = 0;
        StatusPack.saveSphere = StatusPack.lifeSphereCount;
        panels[0].text.text = StatusPack.money.ToString();
        panels[1].text.text = StatusPack.lifeSphereCount.ToString();
        panels[2].text.text = StatusPack.acidCount.ToString();
        foreach (var item in panels)
        {
            item.anim.SetBool("Visible", true);
        }
        
        returnTime = 3;
    }
    void Update()
    {
        healtSlider.value = Mathf.Lerp(healtSlider.value, _health, Time.deltaTime * 10);
        if(returnTime - Time.deltaTime > 0)
        {
            returnTime -= Time.deltaTime;
        }
        else
        {
            returnTime = 0;
            Return();
        }
        SpendMoney();
        StatisticInput();
    }
    #endregion

    #region Публичные методы
    public void CheckTexts()
    {
        panels[0].text.text = StatusPack.money.ToString();
        panels[1].text.text = StatusPack.lifeSphereCount.ToString();
        panels[2].text.text = StatusPack.acidCount.ToString();
    }
    public void AddCoin()
    {
        StatusPack.money++;
        CheckTexts();
        panels[0].anim.SetBool("Visible", true);
        returnTime = 3;
    }
    public void AddAcid(float value)
    {
        StatusPack.acidCount+= value;
        StatusPack.acidCount = Mathf.Clamp(StatusPack.acidCount, 0, StatusPack.maxAcidCount);
        CheckTexts();
        panels[2].anim.SetBool("Visible", true);
        returnTime = 3;
        onGetLoot?.Invoke();
    }
    public void AddLifeSphere()
    {
        StatusPack.lifeSphereCount++;
        CheckTexts();
        panels[1].anim.SetBool("Visible", true);
        returnTime = 3;
    }
    public void RemoveLifeSphere()
    {
        StatusPack.lifeSphereCount--;
        CheckTexts();
        panels[1].anim.SetBool("Visible", true);
        returnTime = 3;
    }
    public void CheckSpheres()
    {
        StatusPack.lifeSphereCount = StatusPack.saveSphere > StatusPack.lifeSphereCount ? StatusPack.lifeSphereCount : StatusPack.saveSphere;
    }
    public void SaveStats()
    {
        StatusPack.saveMoney = StatusPack.money;
        StatusPack.saveAcidCount = StatusPack.acidCount;
        StatusPack.saveSphere = StatusPack.lifeSphereCount;
    }
    public void Return()
    {
        foreach (var item in panels)
        {
            item.anim.SetBool("Visible", false);
        }
    }
    public void SetTip(LootPointScript lootPoint, InputSettingsManager manager)
    {
        string result = string.Empty;
        if(lootPoint.cost > StatusPack.money)
        {
            tip.text = "Не хватает монет для покупки " + lootPoint.tipText;
        }
        else
        {
            tip.text = "Нажмите " + manager.GetKey("Using").ToString() + ", чтобы купить " + lootPoint.tipText;   
        }
    }
    public void ClearTip()
    {
        tip.text = string.Empty;
    }
    public void SpendMoney(int count)
    {
        StatusPack.money -= count;
        spendMoney = true;
    }
    #endregion

    #region Служебные методы
    private void ReturnSpend() => spendMoney = true;
    private void SpendMoney()
    {
        if(spendMoney)
        {
            int currentCoinCountInText = int.Parse(panels[0].text.text);
            if (currentCoinCountInText > StatusPack.money)
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
    private void StatisticInput()
    {
        if(Input.GetKeyDown(manager.GetKey("Info")))
        {
            foreach (var item in panels)
            {
                item.anim.SetBool("Visible", true);
            }
            returnTime = 3;
        }
    }
    #endregion
}
