using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;

    private NetworkVariable<bool> isWhiteTurn = new NetworkVariable<bool>(true); // White starts

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public bool IsMyTurn(bool isWhite)
    {
        return isWhite == isWhiteTurn.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc()
    {
        if (!IsServer) return;
        isWhiteTurn.Value = !isWhiteTurn.Value;
        Debug.Log($"Turn changed. It's now {(isWhiteTurn.Value ? "White's" : "Black's")} turn.");
    }
}


