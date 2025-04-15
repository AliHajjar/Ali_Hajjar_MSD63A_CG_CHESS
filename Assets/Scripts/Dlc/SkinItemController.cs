using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinItemController : MonoBehaviour
{
    [SerializeField] private string skinName;
    [SerializeField] private string skinURL;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private ProfileSkinLoader profileSkinLoader;

    private void Start()
    {
        buyButton.onClick.AddListener(HandleBuy);
        equipButton.onClick.AddListener(HandleEquip);

        UpdateUI();
    }

    private void HandleBuy()
    {
        PurchasedSkinManager.Instance.MarkAsPurchased(skinName);
        Debug.Log($"Purchased {skinName}");
        UpdateUI();
    }

    private void HandleEquip()
    {
        if (!PurchasedSkinManager.Instance.IsPurchased(skinName))
        {
            Debug.Log("You must purchase this first.");
            return;
        }

        ulong myId = Unity.Netcode.NetworkManager.Singleton.LocalClientId;

        if (myId == 0) // Host
        {
            GameObject.Find("player1").GetComponent<ProfileSkinLoader>().LoadSkinFromURL(skinURL);
        }
        else // Client
        {
            GameObject.Find("player2").GetComponent<ProfileSkinLoader>().LoadSkinFromURL(skinURL);
        }

        Debug.Log($"Equipped {skinName} to Player {(myId == 0 ? "1" : "2")}");
    }


    private void UpdateUI()
    {
        bool purchased = PurchasedSkinManager.Instance.IsPurchased(skinName);
        buyButton.gameObject.SetActive(!purchased);
        equipButton.gameObject.SetActive(purchased);
    }
}