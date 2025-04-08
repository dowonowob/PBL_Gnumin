using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class LightEstimationManager : MonoBehaviour
{
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] private Material sharedMaterial; // 오브젝트에 적용할 머티리얼

    private void OnEnable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.colorCorrection.HasValue && sharedMaterial != null)
        {
            sharedMaterial.color = args.lightEstimation.colorCorrection.Value;
        }

        if (args.lightEstimation.averageBrightness.HasValue)
        {
            float intensity = args.lightEstimation.averageBrightness.Value;
            sharedMaterial.SetFloat("_Glossiness", Mathf.Clamp01(intensity)); // 예시
        }
    }
}
