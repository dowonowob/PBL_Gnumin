using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;

public class SmoothNavMeshFollower_Curve : MonoBehaviour
{
    [Header("References")]
    public LightshipNavMeshAgent curveAgent;

    [Header("Movement Settings")]
    public float curveMoveSpeed = 4.0f;
    public float curveRotationSmoothTime = 0.2f;
    public float curveWaypointThreshold = 0.2f;

    private int curveWaypointIndex = 0;
    private Vector3 curveCurrentVelocity;
    private Vector3 curveSmoothedDirection;

    private void Start()
    {
        curveSmoothedDirection = transform.forward;
    }

    private void Update()
    {
        if (curveAgent == null || curveAgent.path == null || curveAgent.path.Waypoints.Count == 0)
            return;

        if (curveWaypointIndex >= curveAgent.path.Waypoints.Count)
            curveWaypointIndex = curveAgent.path.Waypoints.Count - 1;

        Vector3 curveTargetPosition = curveAgent.path.Waypoints[curveWaypointIndex].WorldPosition;
        Vector3 directionToCurve = curveTargetPosition - transform.position;
        directionToCurve.y = 0;

        float curveDistance = directionToCurve.magnitude;

        if (curveDistance <= curveWaypointThreshold)
        {
            if (curveWaypointIndex < curveAgent.path.Waypoints.Count - 1)
                curveWaypointIndex++;
            return;
        }

        Vector3 desiredCurveDirection = directionToCurve.normalized;

        // 이동 방향 부드럽게
        curveSmoothedDirection = Vector3.SmoothDamp(curveSmoothedDirection, desiredCurveDirection, ref curveCurrentVelocity, curveRotationSmoothTime);

        // 회전 부드럽게
        if (curveSmoothedDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(curveSmoothedDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // 이동
        transform.position += curveSmoothedDirection * curveMoveSpeed * Time.deltaTime;
    }

    private void OnEnable()
    {
        curveWaypointIndex = 0;
        curveSmoothedDirection = transform.forward;
    }
}
