using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityChess;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;

    // Track whose turn it is (true = white, false = black)
    private NetworkVariable<bool> isWhiteTurn = new NetworkVariable<bool>(true);

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            // Only clients listen to value changes
            isWhiteTurn.OnValueChanged += OnTurnChanged;
        }
        else
        {
            // Server forces update just in case
            NotifyTurnChangedClientRpc(isWhiteTurn.Value);
        }
    }

    /// <summary>
    /// Determines if it's the local player's turn based on color.
    /// </summary>
    public bool IsMyTurn(bool isWhite)
    {
        return isWhite == isWhiteTurn.Value;
    }

    /// <summary>
    /// Called by the server to end the current turn and switch to the other player.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc()
    {
        if (!IsServer) return;

        isWhiteTurn.Value = !isWhiteTurn.Value;
        Debug.Log($"[Server] Turn changed. It's now {(isWhiteTurn.Value ? "White's" : "Black's")} turn.");

        // Explicitly notify clients to refresh interactivity
        NotifyTurnChangedClientRpc(isWhiteTurn.Value);
    }

    private void OnEnable()
    {
        isWhiteTurn.OnValueChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        isWhiteTurn.OnValueChanged -= OnTurnChanged;
    }

    /// <summary>
    /// When the server updates turn, clients get notified here.
    /// </summary>
    private void OnTurnChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"[Client] Turn changed to: {(newValue ? "White" : "Black")}");

        if (!IsServer)
        {
            // Client side: Refresh piece interactivity
            BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(newValue ? Side.White : Side.Black);
        }
    }

    [ClientRpc]
    private void NotifyTurnChangedClientRpc(bool isWhiteTurnNow)
    {
        Debug.Log($"[ClientRpc] Sync turn: It is now {(isWhiteTurnNow ? "White's" : "Black's")} turn.");

        // Refresh interactivity on all clients
        BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(isWhiteTurnNow ? Side.White : Side.Black);
    }
}
