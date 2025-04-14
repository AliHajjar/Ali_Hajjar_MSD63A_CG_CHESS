using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class ConnectionManager : MonoBehaviour
{
    private void Start()
    {
        // Optional: simulate handling a bad connection attempt
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
    }

    private void OnClientStopped(bool wasClean)
    {
        if (!wasClean)
        {
            Debug.LogWarning("Client stopped unexpectedly. Likely due to connection failure.");
 
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");

    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");

    }
}

