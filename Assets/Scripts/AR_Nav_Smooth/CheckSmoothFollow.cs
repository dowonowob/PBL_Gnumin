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
    public float turnSpeed = 5f;        // ȸ�� ���� �ӵ�
    public float reachThreshold = 0.1f; // ���� ��� ����

    private List<Vector3> curvePoints = new List<Vector3>();
    private int curveIndex = 0;

    void Update()
    {
        if (agent == null || agent.path == null) return;

        // 1) ���� ����Ʈ ����: ���� ��ġ + Waypoints.WorldPosition
        var raw = new List<Vector3> { transform.position };
        foreach (var wp in agent.path.Waypoints)
            raw.Add(wp.WorldPosition);

        // 2) Chaikin ������
        curvePoints = Chaikin(raw, smoothIterations);

        // 3) �ε��� ����
        if (curveIndex >= curvePoints.Count)
            curveIndex = curvePoints.Count - 1;

        // 4) �̵� & ȸ��
        Vector3 target = curvePoints[curveIndex];
        Vector3 dir3D = target - transform.position;
        float dist = dir3D.magnitude;

        // Waypoint�� ���������� ���� ����Ʈ��
        if (dist < reachThreshold)
        {
            curveIndex = Mathf.Min(curveIndex + 1, curvePoints.Count - 1);
            return;
        }

        Vector3 desiredDir3D = dir3D.normalized;

        // ȸ���� XZ ��� �������θ� (Y�� ���� ����)
        Vector3 desiredDir2D = new Vector3(desiredDir3D.x, 0f, desiredDir3D.z).normalized;
        if (desiredDir2D.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(desiredDir2D),
                Time.deltaTime * turnSpeed
            );
        }

        // �̵��� ���� 3D ��������
        transform.position += desiredDir3D * (moveSpeed * Time.deltaTime);
    }

    // Chaikin corner-cutting ������
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
