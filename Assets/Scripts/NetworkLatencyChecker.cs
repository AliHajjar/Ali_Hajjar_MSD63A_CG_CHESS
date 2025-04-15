using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class NetworkLatencyChecker : NetworkBehaviour
{
    private float pingInterval = 2f;

    public override void OnNetworkSpawn()
    {
        if (IsClient && !IsServer)
        {
            StartCoroutine(SendPingRoutine());
        }
    }


    private IEnumerator SendPingRoutine()
    {
        while (true)
        {
            float sendTime = Time.time;
            SendPingServerRpc(sendTime);
            yield return new WaitForSeconds(pingInterval);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendPingServerRpc(float clientSendTime, ServerRpcParams rpcParams = default)
    {
        ReturnPingClientRpc(clientSendTime, rpcParams.Receive.SenderClientId);
    }


    [ClientRpc(RequireOwnership = false)]
    private void ReturnPingClientRpc(float originalClientTime, ulong clientId)
    {
        if (!IsOwner || NetworkManager.Singleton.LocalClientId != clientId)
            return;

        float rtt = (Time.time - originalClientTime) * 1000f; // convert to ms
        Debug.Log($"[Latency] RTT to server: {rtt:F1} ms (ClientId: {NetworkManager.Singleton.LocalClientId})");

    }
}
