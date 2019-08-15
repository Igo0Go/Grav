using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
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
    #endregion

    public float Health { get { return _health; } set { _health = value; } }
    private float _health;
    private float returnTime;

    void Start()
    {
        _health = 100;
        healtSlider.value = 0;
        foreach (var item in panels)
        {
            item.anim.SetBool("Visible", false);
            item.text.text = "0";
        }
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
    }

    public void CheckTexts()
    {
        panels[0].text.text = StatusPack.money.ToString();
        panels[1].text.text = StatusPack.lifeSphereCount.ToString();
    }
    public void AddCoin()
    {
        StatusPack.money++;
        CheckTexts();
        panels[0].anim.SetBool("Visible", true);
        returnTime = 3;
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
    public void Return()
    {
        foreach (var item in panels)
        {
            item.anim.SetBool("Visible", false);
        }
    }
}
