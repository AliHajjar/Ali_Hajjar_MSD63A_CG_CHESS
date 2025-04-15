using UnityEngine;

public class ResetSkinPurchases : MonoBehaviour
{
    void Start()
    {
        if (PurchasedSkinManager.Instance != null)
        {
            PurchasedSkinManager.Instance.ResetPurchases();
            Debug.Log("Skin purchases reset on game start.");
        }
    }
}
