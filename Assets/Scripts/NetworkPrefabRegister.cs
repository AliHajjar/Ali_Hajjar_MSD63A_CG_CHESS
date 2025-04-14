using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPrefabRegister : MonoBehaviour
{
    public GameObject playerPrefab; // Assign in Inspector

    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found in scene!");
            return;
        }

        var config = NetworkManager.Singleton.NetworkConfig;

        if (playerPrefab != null && !config.Prefabs.Contains(playerPrefab))
        {
            config.Prefabs.Add(new NetworkPrefab()
            {
                Prefab = playerPrefab
            });

            Debug.Log("Player prefab registered at runtime.");
        }
        else if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned!");
        }
    }

}
