using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;

public class SmoothNavMeshFollower : MonoBehaviour
{
    [Header("References")]
    public LightshipNavMeshAgent targetAgent;

    [Header("Movement Settings")]
    public float linearMoveSpeed = 4.0f;
    public float linearRotationSpeed = 5.0f;
    public float waypointReachThreshold = 0.2f;

    private int linearWaypointIndex = 0;

    private void Update()
    {
        if (targetAgent == null || targetAgent.path == null || targetAgent.path.Waypoints.Count == 0)
            return;

        if (linearWaypointIndex >= targetAgent.path.Waypoints.Count)
            linearWaypointIndex = targetAgent.path.Waypoints.Count - 1;

        Vector3 waypointTarget = targetAgent.path.Waypoints[linearWaypointIndex].WorldPosition;
        Vector3 directionToWaypoint = waypointTarget - transform.position;
        directionToWaypoint.y = 0;

        float distance = directionToWaypoint.magnitude;

        if (distance <= waypointReachThreshold)
        {
            if (linearWaypointIndex < targetAgent.path.Waypoints.Count - 1)
                linearWaypointIndex++;
            return;
        }

        Vector3 normalizedDirection = directionToWaypoint.normalized;

        // 방향 회전 보간
        Vector3 smoothDirection = Vector3.Lerp(transform.forward, normalizedDirection, Time.deltaTime * linearRotationSpeed);
        transform.rotation = Quaternion.LookRotation(smoothDirection);

        // 이동
        transform.position += smoothDirection * linearMoveSpeed * Time.deltaTime;
    }

    private void OnEnable()
    {
        linearWaypointIndex = 0;
    }
}
