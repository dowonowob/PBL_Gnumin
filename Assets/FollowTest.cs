using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.Utilities;
using UnityEngine;

namespace Niantic.Lightship.AR.NavigationMesh
{

    public class FollowDeviceViaNavMesh : MonoBehaviour
    {
        private LightshipNavMeshAgent agent;
        private Camera cam;
        private float updateInterval = 0.5f;
        private float timer;

        private void Start()
        {
            agent = GetComponent<LightshipNavMeshAgent>();
            cam = Camera.main;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval && agent.State == LightshipNavMeshAgent.AgentNavigationState.Idle)
            {
                timer = 0f;
                Vector3 targetPos = cam.transform.position;
                agent.SetDestination(targetPos);
            }
        }
    }
}
