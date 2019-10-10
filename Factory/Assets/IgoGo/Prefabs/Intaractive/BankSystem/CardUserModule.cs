using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUserModule : MonoBehaviour
{
    public LayerMask ignoreMask;
    public Transform cam;

    private InputSettingsManager settingsManager;
    private GravFPSUI gravFPSUI;
    private RaycastHit hit;

    void Start()
    {
        settingsManager = GetComponent<InputSettingsManager>();
        gravFPSUI = GetComponent<GravFPSUI>();
    }
    void Update()
    {
        CheckItem();
    }

    private void CheckItem()
    {
        if(Physics.Raycast(cam.position , cam.forward, out hit, 1.4f, ~ignoreMask))
        {
            if(hit.collider.tag.Equals("CardPoint"))
            {
                BankCard card = hit.collider.GetComponent<BankCard>();
                if(gravFPSUI.StatusPack.cards[card.number])
                {
                    gravFPSUI.SetTip(card.tipText);
                    if (Input.GetKeyDown(settingsManager.GetKey("Using")))
                    {
                        card.InstanceCard();
                        gravFPSUI.RemoveCard(card.number);
                        gravFPSUI.ClearTip();
                        return;
                    }
                }
                else
                {
                    gravFPSUI.SetTip("Нет нужной карты");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("CardPoint"))
        {
            gravFPSUI.SetTip(settingsManager.GetKey("Using").ToString() + " - ввести код");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("CardPoint"))
        {
            gravFPSUI.ClearTip();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag.Equals("CardPoint"))
        {
            if (Input.GetKeyDown(settingsManager.GetKey("Using")))
            {
                gravFPSUI.ClearTip();
                other.GetComponent<ImageCodePanel>().SetPlayer(GetComponent<GravFPS>());
            }
        }
    }
}
