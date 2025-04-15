using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;

public class NavMeshSyncController : MonoBehaviour
{
    public GameObject character; // 따라오는 캐릭터
    public Transform meshManager; // ARMeshManager 또는 Meshing 컴포넌트 위치
    public LightshipNavMeshManager navMeshManager; // ARDK NavMesh Manager

    [Tooltip("몇 초마다 NavMeshManager 위치와 재빌드를 갱신할지")]
    public float updateInterval = 1.5f;

    private float timer = 0f;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (character == null || meshManager == null || navMeshManager == null || mainCam == null)
            return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;

            // 사용자(카메라)와 캐릭터 사이 중간 위치 계산
            Vector3 mid = (mainCam.transform.position + character.transform.position) / 2f;

            // 메싱 중심 위치 이동
            meshManager.position = mid;

            // NavMesh 중심 위치 이동 및 재빌드
            navMeshManager.transform.position = mid;

        }
    }
}
