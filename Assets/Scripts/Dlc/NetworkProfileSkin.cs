using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

/// <summary>
/// Handles syncing of a player's profile image (avatar skin) over the network.
/// Uses a NetworkVariable to keep all clients updated in real-time.
/// </summary>
public class NetworkProfileSkin : NetworkBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image profileImage; // This is the UI image that will show the skin

    private ProfileSkinLoader skinLoader;

    // NetworkVariable to hold the equipped skin URL
    private NetworkVariable<FixedString512Bytes> equippedSkinUrl = new NetworkVariable<FixedString512Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private void Awake()
    {
        skinLoader = GetComponent<ProfileSkinLoader>();
    }


    public override void OnNetworkSpawn()
    {
        equippedSkinUrl.OnValueChanged += OnSkinChanged;

        // Show current skin if already set
        if (!string.IsNullOrEmpty(equippedSkinUrl.Value.ToString()))
        {
            ApplySkin(equippedSkinUrl.Value.ToString());
        }
    }

    private void OnSkinChanged(FixedString512Bytes oldUrl, FixedString512Bytes newUrl)
    {
        Debug.Log($"Skin changed to: {newUrl}");
        ApplySkin(newUrl.ToString());
    }

    private void ApplySkin(string url)
    {
        if (profileImage != null)
        {
            profileImage.enabled = true;
        }

        if (skinLoader != null)
        {
            skinLoader.LoadSkinFromURL(url);
        }
    }

    /// <summary>
    /// Call this method to change your skin (only if you're the owner of this object).
    /// </summary>
    public void EquipSkin(string url)
    {
        if (!IsOwner)
        {
            Debug.LogWarning("Only the owner can equip a skin on this profile.");
            return;
        }

        if (url.Length > 500)
        {
            Debug.LogError("Skin URL too long for FixedString512Bytes.");
            return;
        }

        equippedSkinUrl.Value = url;
    }
}
