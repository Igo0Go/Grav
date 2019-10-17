using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class LootPointScript : UsingOrigin
{
    [Tooltip("Предмет, который будет выпадывать")] public GameObject lootPrefab;
    [Tooltip("Монетка без скриптов")] public List<GameObject> spendPrefab;
    [Tooltip("Куда будет выпадывать предмет")] public Transform lootPoint;
    [Tooltip("Куда выводить цену")] public Text costText;
    [Tooltip("Цена предмета")] public int cost;
    [Tooltip("Подсказка - название предемета")] public string tipText;
    public bool coinsType;
    [HideInInspector] public byte cardContains;

    [HideInInspector] public bool useble;
    private GameObject bufer;
    private Transform player;
    private GravFPSUI gravFPSUI;
    private List<Transform> spendObjects;
    private bool spawn;
    private bool usingOrigin;
    private byte currentSpendIndex;

    private void OnEnable()
    {
        useble = true;
        spendObjects = new List<Transform>();
        gameObject.tag = "LootPoint";
    }

    void Start()
    {
        ToStart();
    }
    void Update()
    {
        CheckSpawn();
        MoveCoins();
    }

    public void SetPlayer(GravFPSUI fPS)
    {
        spendObjects = new List<Transform>();
        currentSpendIndex = 0;
        gravFPSUI = fPS;
        player = gravFPSUI.transform;
        spawn = true;
    }
    public override void Use()
    {
        if(usingOrigin)
        {
            UseAll();
        }
        else
        {
            Instantiate(lootPrefab, lootPoint.position, Quaternion.identity, transform);
        }
    }
    public override void ToStart()
    {
        spendObjects = new List<Transform>();
        costText.text = cost.ToString();
        usingOrigin = lootPrefab == null;
        if(coinsType)
        {
            cardContains = 0;
        }
    }

    private void CheckSpawn()
    {
        if (spawn)
        {
            if(coinsType)
            {
                if (spendObjects.Count < cost)
                {
                    spendObjects.Add(Instantiate(spendPrefab[0], player.position, Quaternion.identity, transform).transform);
                    Invoke("ReturnSpawn", 0.1f);
                }
                else
                {
                    if (usingOrigin)
                    {
                        UseAll();
                    }
                    else
                    {
                        Instantiate(lootPrefab, lootPoint.position, Quaternion.identity, transform);
                        useble = true;
                    }
                }
            }
            else
            {
                if (currentSpendIndex < 4)
                {
                    if(gravFPSUI.StatusPack.cards[currentSpendIndex])
                    {
                        gravFPSUI.StatusPack.cards[currentSpendIndex] = false;
                        spendObjects.Add(Instantiate(spendPrefab[currentSpendIndex], player.position, Quaternion.identity, transform).transform);
                        cardContains++;
                    }
                    currentSpendIndex++;
                    Invoke("ReturnSpawn", 0.1f);
                }
                else if(cardContains > 3)
                {
                    if (usingOrigin)
                    {
                        UseAll();
                    }
                    else
                    {
                        Instantiate(lootPrefab, lootPoint.position, Quaternion.identity, transform);
                        useble = true;
                    }
                }
                else
                {
                    useble = true;
                }
            }
        }
        spawn = false;
    }
    private void ReturnSpawn() => spawn = true;
    private void MoveCoins()
    {
        for (int i = 0; i < spendObjects.Count; i++)
        {
            Transform coin = spendObjects[i];
            if(coin != null)
            {
                if (Vector3.Distance(coin.position, transform.position) > 0.3f)
                {
                    coin.position = Vector3.Lerp(coin.position, transform.position, Time.deltaTime * 5);
                }
                else
                {
                    Destroy(coin.gameObject);
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
