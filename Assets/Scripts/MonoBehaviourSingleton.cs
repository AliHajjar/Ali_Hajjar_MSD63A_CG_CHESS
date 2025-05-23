﻿using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
