using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;

namespace Niantic.Lightship.AR.NavigationMesh
{
    public class FollowDeviceViaNavMesh : MonoBehaviour
    {
        private NavMeshAgent_Custom agent;
        private Camera cam;

        private Vector3 lastTargetPosition;
        private float moveThreshold = 0.2f; // 20cm 이상 이동할 때만 재탐색
        private float checkInterval = 0.1f;
        private float timer;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent_Custom>();
            cam = Camera.main;
            lastTargetPosition = cam.transform.position;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= checkInterval)
            {
                timer = 0f;
                Vector3 currentTargetPos = cam.transform.position;
                float distanceMoved = Vector3.Distance(currentTargetPos, lastTargetPosition);

                if (distanceMoved > moveThreshold && agent.State == NavMeshAgent_Custom.AgentNavigationState.Idle)
                {
                    agent.SetDestination(currentTargetPos);
                    lastTargetPosition = currentTargetPos;
                }
            }
        }
    }
}
