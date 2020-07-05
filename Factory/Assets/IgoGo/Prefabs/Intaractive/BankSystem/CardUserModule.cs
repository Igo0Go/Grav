using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerUIController))]
public class CardUserModule : MonoBehaviour
{
    public LayerMask ignoreMask;
    public Transform cam;

    private InputSettingsManager settingsManager;
    private PlayerUIController playerUIController;
    private RaycastHit hit;

    void Start()
    {
        settingsManager = GetComponent<InputSettingsManager>();
        playerUIController = GetComponent<PlayerUIController>();
    }
    

    private void CheckItem()
    {
        if(Physics.Raycast(cam.position , cam.forward, out hit, 1.4f, ~ignoreMask))
        {
            if(hit.collider.CompareTag("CardPoint"))
            {
                BankCard card = hit.collider.GetComponent<BankCard>();
                if(playerUIController.StatusPack.cards[card.number])
                {
                    playerUIController.SetTip(card.tipText, true);
                    if (Input.GetKeyDown(settingsManager.GetKey("Using")))
                    {
                        card.InstanceCard();
                        playerUIController.RemoveCard(card.number);
                        playerUIController.ClearTip();
                        return;
                    }
                }
                else
                {
                    playerUIController.SetTip("Нет нужной карты", false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CardPoint"))
        {
            playerUIController.SetTip("ввести код", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CardPoint"))
        {
            playerUIController.ClearTip();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("CardPoint"))
        {
            if (Input.GetKeyDown(settingsManager.GetKey("Using")))
            {
                playerUIController.ClearTip();
                other.GetComponent<ImageCodePanel>().SetPlayer(playerUIController.PlayerStateController);
            }
        }
    }
}
