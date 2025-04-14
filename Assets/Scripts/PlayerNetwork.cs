using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    public bool isWhite;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            isWhite = IsServer; // Host = White, Client = Black
            Debug.Log($"You are {(isWhite ? "White" : "Black")}");
        }
    }
}