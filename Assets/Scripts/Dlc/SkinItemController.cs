using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System.Threading.Tasks;

using UnityEngine.Analytics;


public class SkinItemController : MonoBehaviour
{
    [SerializeField] private string skinName;
    [SerializeField] private string skinURL;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private ProfileSkinLoader profileSkinLoader;

    private async void Start()
    {
        // Initialize Unity Services only once per session
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        buyButton.onClick.AddListener(HandleBuy);
        equipButton.onClick.AddListener(HandleEquip);

        UpdateUI();
    }

    private void HandleBuy()
    {
        PurchasedSkinManager.Instance.MarkAsPurchased(skinName);
        Debug.Log($"Purchased {skinName}");

        // Log DLC purchase
        Analytics.CustomEvent("dlc_purchase", new Dictionary<string, object>
        {
            { "skin", skinName },
            { "timestamp", System.DateTime.UtcNow.ToString("o") },
            { "userId", SystemInfo.deviceUniqueIdentifier }
        });

        UpdateUI();
    }


    private void HandleEquip()
    {
        if (PurchasedSkinManager.Instance.IsPurchased(skinName))
        {
            var netSkin = FindObjectOfType<NetworkProfileSkin>(); // You can assign this via Inspector too
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
