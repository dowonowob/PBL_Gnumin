using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;

public class NavMeshSyncController : MonoBehaviour
{
    public GameObject character; // ������� ĳ����
    public Transform meshManager; // ARMeshManager �Ǵ� Meshing ������Ʈ ��ġ
    public LightshipNavMeshManager navMeshManager; // ARDK NavMesh Manager

    [Tooltip("�� �ʸ��� NavMeshManager ��ġ�� ����带 ��������")]
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

            // �����(ī�޶�)�� ĳ���� ���� �߰� ��ġ ���
            Vector3 mid = (mainCam.transform.position + character.transform.position) / 2f;

            // �޽� �߽� ��ġ �̵�
            meshManager.position = mid;

            // NavMesh �߽� ��ġ �̵� �� �����
            navMeshManager.transform.position = mid;

        }
    }
}
