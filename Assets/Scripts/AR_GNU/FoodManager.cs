using System.Collections.Generic;
using UnityEngine;

public class SphereManager : MonoBehaviour
{
    public static SphereManager Instance { get; private set; }

    private readonly Queue<Transform> _sphereQueue = new Queue<Transform>();
    private const int MaxSpheres = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterSphere(Transform sphere)
    {
        if (_sphereQueue.Count >= MaxSpheres)
        {
            var oldSphere = _sphereQueue.Dequeue();
            if (oldSphere != null) Destroy(oldSphere.gameObject);
        }

        _sphereQueue.Enqueue(sphere);
        AgentChasePlayer.Instance.SetTarget(_sphereQueue.Peek());
    }

    public void RemoveSphere(Transform sphere)
    {
        if (_sphereQueue.Count > 0 && _sphereQueue.Peek() == sphere)
        {
            _sphereQueue.Dequeue();
            if (_sphereQueue.Count > 0)
                AgentChasePlayer.Instance.SetTarget(_sphereQueue.Peek());
            else
                AgentChasePlayer.Instance.ClearTarget();
        }
    }
}