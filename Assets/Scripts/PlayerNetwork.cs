using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<bool> IsWhite = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            IsWhite.Value = IsServer; // Host is white, client is black
            Debug.Log($"You are {(IsWhite.Value ? "White" : "Black")}");
        }
    }
}
