using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UsingObject: MyTools
{
    [Tooltip("Отрисовка в редакторе")] public bool debug;

    [HideInInspector] public bool used;
    public abstract void Use();
    public abstract void ToStart();
}

public abstract class UsingOrigin : UsingObject
{
    [Tooltip("Объекты, у которых будет вызываться метод USE()")] public List<UsingObject> actionObjects;
}

public class ActionObject : UsingOrigin {

    [Space(20)]
    [Tooltip("Здесь будет выводиться текстовая подсказка")] public Text tip;
    [Tooltip("Не обязательный. Показывает значение энергии. У аниматора должен быть параметр bool ConnectEnergy")]
    public Animator energyMarker;
    public bool Active
    {
        get { return _active; }
        set
        {
            if(energyMarker != null)
            {
                energyMarker.SetBool("ConnectEnergy", value);
            }
            _active = value;
        }
    }

    [Space(10)]
    [Tooltip("Активно изначально")]
    public bool startActive;

    private bool _active;
    private bool key;
    private string tipText;

    private void Start()
    {
        Active = startActive;
        key = true;
    }

    public void OnChangeActiveHandler(bool value)
    {
        if(tipText == null)
        {
            tipText = tip.text;
        }
        Active = value;
        if (Active)
        {
            tip.text = tipText;
        }
        else
        {
            tip.text = "Нет энергии";
        }
    }
    public override void Use()
    {
        if(key && Active)
        {
            UseAll();
            key = false;
            used = true;
            Invoke("ResetActive", 1f);
        }
    }
    public override void ToStart()
    {
        ResetActive();
    }
    public void UseAll()
    {
        for (int i = 0; i < actionObjects.Count; i++)
        {
            if (actionObjects[i] != null)
            {
                actionObjects[i].Use();
            }
            else
            {
                Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
            }
        }
    }
    private void ResetActive()
    {
        used = false;
        key = true;
    }
}
