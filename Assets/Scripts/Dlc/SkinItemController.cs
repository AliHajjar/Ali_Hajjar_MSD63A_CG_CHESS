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
        if (PurchasedSkinManager.Instance.IsPurchased(skinName))
        {
            var netSkin = FindObjectOfType<NetworkProfileSkin>(); // Or assign via inspector
            netSkin?.EquipSkin(skinURL);
        }
        else
        {
            Debug.Log("You must purchase this first.");
        }
    }



    private void UpdateUI()
    {
        bool purchased = PurchasedSkinManager.Instance.IsPurchased(skinName);
        buyButton.gameObject.SetActive(!purchased);
        equipButton.gameObject.SetActive(purchased);
    }
}