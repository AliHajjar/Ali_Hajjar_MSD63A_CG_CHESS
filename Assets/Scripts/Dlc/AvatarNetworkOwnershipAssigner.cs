using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AvatarNetworkOwnershipAssigner : NetworkBehaviour
{
    [SerializeField] private NetworkObject hostAvatar;
    [SerializeField] private NetworkObject clientAvatar;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Host owns bottom (player2), client owns top (player1)
            hostAvatar.ChangeOwnership(NetworkManager.Singleton.LocalClientId);

            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientId != NetworkManager.Singleton.LocalClientId)
                {
                    clientAvatar.ChangeOwnership(clientId);
                }
            }
        }
    }
}
