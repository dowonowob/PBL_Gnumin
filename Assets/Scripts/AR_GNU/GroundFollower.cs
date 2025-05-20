using UnityEngine;

public class GroundFollowerPlane : MonoBehaviour
{
    public float distanceFromCamera = 1.5f;
    public float yOffset = -0.05f;
    public Material debugMaterial;

    private Transform cam;
    private GameObject planeObject;

    void Start()
    {
        cam = Camera.main?.transform;
        if (cam == null)
        {
            Debug.LogError("Camera.main not found. Make sure your ARCamera has the 'MainCamera' tag.");
            return;
        }

        planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeObject.name = "GeneratedGroundPlane";
        planeObject.transform.SetParent(transform); // �θ�-�ڽ� ������ �������

        planeObject.transform.localScale = new Vector3(1.5f, 1f, 1.5f); // 15x15 ����
        planeObject.layer = LayerMask.NameToLayer("Ground");
        planeObject.tag = "Ground";

        var renderer = planeObject.GetComponent<Renderer>();
        if (renderer != null && debugMaterial != null)
        {
            renderer.enabled = true;
            renderer.material = debugMaterial;
        }

        // Collider ����
        var collider = planeObject.GetComponent<Collider>();
        if (collider == null)
        {
            planeObject.AddComponent<MeshCollider>();
        }
    }

    void Update()
    {
        if (cam == null || planeObject == null) return;

        Vector3 forward = cam.forward;
        Vector3 targetPos = cam.position + forward * distanceFromCamera;
        targetPos.y += yOffset;

        transform.position = targetPos;
        transform.rotation = Quaternion.identity; // �θ�� ȸ�� ����
    }
}
