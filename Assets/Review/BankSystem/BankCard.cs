using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankCard : UsingOrigin
{
    public int number;
    public GameObject prefab;
    public string tipText;

    public void InstanceCard()
    {
        Instantiate(prefab, transform).transform.position = transform.position;
        tag = "Untagged";
        Use();
    }

    public override void ToStart()
    {
    }
    public override void Use()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
}
