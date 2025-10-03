using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomMainThreadDispatcher : MonoBehaviour
{
    private static CustomMainThreadDispatcher _instance = null;
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public static CustomMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            throw new Exception("CustomMainThreadDispatcher is not initialized. Please place it in the scene.");
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue()?.Invoke();
            }
        }
    }

    public void Enqueue(Action action)
    {
        if (action == null) return;
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}