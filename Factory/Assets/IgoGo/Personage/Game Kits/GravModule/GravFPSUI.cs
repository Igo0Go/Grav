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
    public Image stunPanel;
    [Range(0.1f,2)] public float stunSpeed;
    [HideInInspector] public InputSettingsManager manager;
    [HideInInspector] public float returnTime;
    public InputKit inputKit;
    #endregion

    #region Приватные поля
    private bool spendMoney;
    #endregion

    #region Свойства
    public float Health { get { return _health; } set { _health = value; } }
    private float _health;
    #endregion

    #region Делегаты и события
    public event Action OnGetLoot;
    public event Action OnFinalStun;
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
        ChangeStunPanel();
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
        OnGetLoot?.Invoke();
    }
    public void AddLifeSphere()
    {
        StatusPack.lifeSphereCount++;
        CheckTexts();
        panels[1].anim.SetBool("Visible", true);
        returnTime = 3;
    }
    public void AddBankCard(int number)
    {
        StatusPack.cards[number] = true;
    }
    public void RemoveCard(int number)
    {
        StatusPack.cards[number] = false;
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
        StatusPack.saveCards = new List<bool>();
        foreach (var item in StatusPack.cards)
        {
            StatusPack.saveCards.Add(item);
        }
        DataLoader.SaveXML(StatusPack, inputKit);
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
        if(lootPoint.cost > StatusPack.money)
        {
            tip.text = "Не хватает монет";
        }
        else
        {
            tip.text = "Нажмите " + manager.GetKey("Using").ToString() + " - " + lootPoint.tipText;   
        }
    }
    public void SetTip(string tipText)
    {
        tip.text = tipText;
    }
    public void ClearTip()
    {
        tip.text = string.Empty;
    }
    public void Spend(int count)
    {
        if(!spendMoney)
        {
            StatusPack.money -= count;
            spendMoney = true;
        }
    }
    public void SetStun()
    {
        stunPanel.gameObject.SetActive(true);
        stunPanel.color = new Color(stunPanel.color.r, stunPanel.color.g, stunPanel.color.b, 1);
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
    private void ChangeStunPanel()
    {
        if(stunPanel.gameObject.activeSelf)
        {
            if(stunPanel.color.a - Time.deltaTime*stunSpeed> 0)
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
    #endregion
}
