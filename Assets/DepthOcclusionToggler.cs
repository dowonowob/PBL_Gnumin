using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[RequireComponent(typeof(Renderer))]
public class DepthOcclusionToggler : MonoBehaviour
{
    public AROcclusionManager occlusionManager;
    public float checkInterval = 0.1f;
    public float margin = 0.05f; // �󸶳� �� ��������� ��Ÿ����

    private Renderer targetRenderer;
    private Camera mainCam;
    private float timer = 0f;

    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        mainCam = Camera.main;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckOcclusion();
        }
    }

    void CheckOcclusion()
    {
        if (occlusionManager == null || mainCam == null)
            return;

        // ĳ������ ȭ�� ��ǥ ���
        Vector3 screenPoint = mainCam.WorldToScreenPoint(transform.position);

        if (screenPoint.z < 0) return; // ī�޶� �ڿ� ������ ����

        if (!occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
            return;

        using (image)
        {
            if (image.format != XRCpuImage.Format.DepthFloat32) return;

            // ȭ����ǥ �� �̹��� ��ǥ ��ȯ
            int imgX = Mathf.Clamp(Mathf.RoundToInt(screenPoint.x / Screen.width * image.width), 0, image.width - 1);
            int imgY = Mathf.Clamp(Mathf.RoundToInt(screenPoint.y / Screen.height * image.height), 0, image.height - 1);

            XRCpuImage.Plane depthPlane = image.GetPlane(0);
            int index = imgY * depthPlane.rowStride / sizeof(float) + imgX;

            unsafe
            {
                float* depthData = (float*)depthPlane.data.GetUnsafePtr();
                float realWorldDepth = depthData[index]; // ���� �繰�� ����
                float virtualDepth = Vector3.Distance(mainCam.transform.position, transform.position); // ĳ���� ����

                // ��
                bool shouldShow = virtualDepth <= realWorldDepth - margin;
                targetRenderer.enabled = shouldShow;
            }
        }
    }
}
