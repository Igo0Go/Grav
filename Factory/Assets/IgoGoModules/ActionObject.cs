using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UsingObject: MyTools
{
    public abstract void Use();
    [Tooltip("Отрисовка в редакторе")] public bool debug;
}

public abstract class UsingOrigin : UsingObject
{
    [Tooltip("Объекты, у которых будет вызываться метод USE()")] public UsingObject[] actionObjects;
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
            Invoke("ResetActive", 1f);
        }
    }
    public void UseAll()
    {
        for (int i = 0; i < actionObjects.Length; i++)
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
        key = true;
    }

}
