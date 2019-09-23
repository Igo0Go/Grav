using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DronModule : UsingOrigin
{
    [Tooltip("Позиция дрона при взломе")]public Transform dronPoint;
    [Tooltip("Время взлома"), Range(0,500)]public float connectTime;
    [Space(), Tooltip("Выводить ли таймер")] public bool drawTimer;
    [Tooltip("Если нужен таймер")] public Text timerText;

    private float timer;

    void Start()
    {
        ToStart();
    }
    private void Update()
    {
        Timer();
    }
    public override void ToStart()
    {
        used = false;
        timer = 0;
        if(drawTimer && timerText)
        {
            timerText.text = string.Empty;
        }
    }
    public override void Use()
    {
        used = true;
    }

    private void Timer()
    {
        if(used)
        {
            if (timer + Time.deltaTime >= connectTime)
            {
                UseAll();
            }
            else
            {
                timer += Time.deltaTime;
                if(drawTimer)
                {
                    timerText.text = timer.ToString();
                }
            }
        }
    }
    private void UseAll()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
}
