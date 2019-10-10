using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class LootPointScript : UsingOrigin
{
    [Tooltip("Предмет, который будет выпадывать")] public GameObject lootPrefab;
    [Tooltip("Монетка без скриптов")] public GameObject coinPrefab;
    [Tooltip("Куда будет выпадывать предмет")] public Transform lootPoint;
    [Tooltip("Куда выводить цену")] public Text costText;
    [Tooltip("Цена предмета")] public int cost;
    [Tooltip("Подсказка - название предемета")] public string tipText;

    [HideInInspector] public bool useble;
    private GameObject bufer;
    private Transform player;
    private GravFPSUI gravFPSUI;
    private List<Transform> coins;
    private bool spawn;
    private bool usingOrigin;

    private void OnEnable()
    {
        useble = true;
        coins = new List<Transform>();
        gameObject.tag = "LootPoint";
    }

    void Start()
    {
        coins = new List<Transform>();
        costText.text = cost.ToString();
        usingOrigin = lootPrefab == null;
    }
    void Update()
    {
        CheckSpawn();
        MoveCoins();
    }

    public void SetPlayer(GravFPSUI fPS)
    {
        coins = new List<Transform>();
        gravFPSUI = fPS;
        player = gravFPSUI.transform;
        spawn = true;
        useble = false;
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

    }

    private void CheckSpawn()
    {
        if (spawn)
        {
            if (coins.Count < cost)

            {
                coins.Add(Instantiate(coinPrefab, player.position, Quaternion.identity, transform).transform);
                Invoke("ReturnSpawn", 0.1f);
            }
            else
            {
                if(usingOrigin)
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
        spawn = false;
    }
    private void ReturnSpawn() => spawn = true;
    private void MoveCoins()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            Transform coin = coins[i];
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
