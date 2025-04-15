using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasedSkinManager : MonoBehaviour
{
    public static PurchasedSkinManager Instance;

    private HashSet<string> purchasedSkins = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPurchases(); // Load saved skins
        }
        else Destroy(gameObject);
    }

    public void MarkAsPurchased(string skinName)
    {
        purchasedSkins.Add(skinName);
        SavePurchases(); // Save after purchase
    }

    public bool IsPurchased(string skinName)
    {
        return purchasedSkins.Contains(skinName);
    }

    // Save to PlayerPrefs
    private void SavePurchases()
    {
        string data = string.Join(",", purchasedSkins);
        PlayerPrefs.SetString("PurchasedSkins", data);
        PlayerPrefs.Save();
    }

    // Load from PlayerPrefs
    private void LoadPurchases()
    {
        if (PlayerPrefs.HasKey("PurchasedSkins"))
        {
            string data = PlayerPrefs.GetString("PurchasedSkins");
            purchasedSkins = new HashSet<string>(data.Split(','));
        }
    }

    public void ResetPurchases()
    {
        purchasedSkins.Clear();
        Debug.Log("All purchased skins have been cleared.");
    }

}
