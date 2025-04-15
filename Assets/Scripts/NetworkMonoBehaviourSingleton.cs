using UnityEngine;
using Unity.Netcode;

public class NetworkMonoBehaviourSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    var go = new GameObject(typeof(T).Name + " (Singleton)");
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }
}
