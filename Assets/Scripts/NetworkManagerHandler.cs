using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class NetworkManagerHandler : MonoBehaviour
{
    void OnGUI()
    {
        // if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            float buttonWidth = 150;
            float buttonHeight = 40;
            float padding = 10;

            float x = Screen.width - buttonWidth - padding;
            float y = padding;

            if (GUI.Button(new Rect(x, y, buttonWidth, buttonHeight), "Start Host"))
                NetworkManager.Singleton.StartHost();

            if (GUI.Button(new Rect(x, y + buttonHeight + padding, buttonWidth, buttonHeight), "Start Client"))
                NetworkManager.Singleton.StartClient();

            if (GUI.Button(new Rect(x, y + 2 * (buttonHeight + padding), buttonWidth, buttonHeight), "Start Server"))
                NetworkManager.Singleton.StartServer();
        }
    }
}
