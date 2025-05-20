using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour
{
    public static ObjManager Instance { get; private set; }

    private readonly Queue<Transform> _objQueue = new Queue<Transform>();
    private const int MaxObj = 3;

    [Header("Ground Plane Control")]
    public GameObject groundPlanePrefab;      
    private GameObject _groundPlaneInstance;  

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterObj(Transform obj)
    {
        if (_objQueue.Count >= MaxObj)
        {
            var oldObj = _objQueue.Dequeue();
            if (oldObj != null)
                Destroy(oldObj.gameObject);
        }

        _objQueue.Enqueue(obj);
        AgentChasePlayer.Instance.SetTarget(_objQueue.Peek());

        if (_groundPlaneInstance == null && groundPlanePrefab != null)
        {
            _groundPlaneInstance = Instantiate(groundPlanePrefab);
        }
    }

    public void RemoveObj(Transform obj)
    {
        if (_objQueue.Count > 0 && _objQueue.Peek() == obj)
        {
            _objQueue.Dequeue();

            if (_objQueue.Count > 0)
                AgentChasePlayer.Instance.SetTarget(_objQueue.Peek());
            else
                AgentChasePlayer.Instance.ClearTarget();
        }

        if (_objQueue.Count == 0 && _groundPlaneInstance != null)
        {
            Destroy(_groundPlaneInstance);
            _groundPlaneInstance = null;
        }
    }
}
