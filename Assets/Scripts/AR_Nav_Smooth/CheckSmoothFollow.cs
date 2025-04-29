using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;

public class ChaikinSmoothNavMeshFollower : MonoBehaviour
{
    [Header("References")]
    public LightshipNavMeshAgent agent;

    [Header("Settings")]
    [Range(1, 5)] public int smoothIterations = 2;
    public float moveSpeed = 4f;        // m/s
    public float turnSpeed = 5f;        // 회전 보간 속도
    public float reachThreshold = 0.1f; // 도달 허용 오차

    private List<Vector3> curvePoints = new List<Vector3>();
    private int curveIndex = 0;

    void Update()
    {
        if (agent == null || agent.path == null) return;

        // 1) 원본 포인트 수집: 현재 위치 + Waypoints.WorldPosition
        var raw = new List<Vector3> { transform.position };
        foreach (var wp in agent.path.Waypoints)
            raw.Add(wp.WorldPosition);

        // 2) Chaikin 스무딩
        curvePoints = Chaikin(raw, smoothIterations);

        // 3) 인덱스 보정
        if (curveIndex >= curvePoints.Count)
            curveIndex = curvePoints.Count - 1;

        // 4) 이동 & 회전
        Vector3 target = curvePoints[curveIndex];
        Vector3 dir3D = target - transform.position;
        float dist = dir3D.magnitude;

        // Waypoint에 도달했으면 다음 포인트로
        if (dist < reachThreshold)
        {
            curveIndex = Mathf.Min(curveIndex + 1, curvePoints.Count - 1);
            return;
        }

        Vector3 desiredDir3D = dir3D.normalized;

        // 회전은 XZ 평면 기준으로만 (Y축 성분 제거)
        Vector3 desiredDir2D = new Vector3(desiredDir3D.x, 0f, desiredDir3D.z).normalized;
        if (desiredDir2D.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(desiredDir2D),
                Time.deltaTime * turnSpeed
            );
        }

        // 이동은 완전 3D 방향으로
        transform.position += desiredDir3D * (moveSpeed * Time.deltaTime);
    }

    // Chaikin corner-cutting 스무딩
    private List<Vector3> Chaikin(List<Vector3> pts, int iterations)
    {
        for (int it = 0; it < iterations; it++)
        {
            var next = new List<Vector3> { pts[0] };
            for (int i = 0; i < pts.Count - 1; i++)
            {
                Vector3 p0 = pts[i], p1 = pts[i + 1];
                Vector3 Q = Vector3.Lerp(p0, p1, 0.25f);
                Vector3 R = Vector3.Lerp(p0, p1, 0.75f);
                next.Add(Q);
                next.Add(R);
            }
            next.Add(pts[pts.Count - 1]);
            pts = next;
        }
        return pts;
    }
}
