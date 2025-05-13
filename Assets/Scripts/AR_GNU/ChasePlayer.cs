using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;

public class AgentChasePlayer : MonoBehaviour
{
    public NavMeshAgent_Custom agent;
    public float updateInterval = 1.0f;

    private Transform cameraTransform;
    private float timer;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (agent == null || cameraTransform == null)
            return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            agent.CustomAgentSetDestination(cameraTransform.position);
            timer = 0f;
        }
    }
}
