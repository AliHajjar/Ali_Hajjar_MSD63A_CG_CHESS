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
        }
        else Destroy(gameObject);
    }

    public void MarkAsPurchased(string skinName)
    {
        purchasedSkins.Add(skinName);
    }

    public bool IsPurchased(string skinName)
    {
        return purchasedSkins.Contains(skinName);
    }
}
