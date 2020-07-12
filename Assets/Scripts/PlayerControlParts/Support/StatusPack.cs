using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/StatusPack")]
public class StatusPack : ScriptableObject
{
    [Tooltip("Номер сохранения")] public int loadSlot;
    [Tooltip("Название текущей сцены")] public string currentScene;
    [Tooltip("Название сцены-хаба")] public string hubScene;

    [Tooltip("Статус загрузки - миссия, хаб, перезагрузка")] public int loadStatus;

    [Tooltip("Текущее количество монет")] public int currentMoneyCount;
    [Tooltip("Текущее количество жизней")] public int currentLifeSphereCount;
    [Tooltip("Текущее количество выстрелов кислоты")] public float currentAcidCount;

    [Tooltip("Сохраннное количество монет")] public int saveMoney;
    [Tooltip("Сохранённое количество жизней")] public int saveSphereCount;
    [Tooltip("Сохранённое количество выстрелов кислоты")] public float saveAcidCount;
    [Tooltip("Максимальное количество выстрелов кислоты в пушке")] public float maxAcidCount;

    [Tooltip("Номер позиции старта в хабе после загрузки")] public int hubPoint;

    [Tooltip("Собранные карты")] public List<bool> cards;
    [Tooltip("Собранные карты")] public List<bool> saveCards;
}
