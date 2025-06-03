using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ObjectTouchHandler : MonoBehaviour
{
    public GameObject uiHUD;
    public GameObject uiInfo;

    public UnityEngine.UI.Image buildingImage;

    public Transform arCamera;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown;
        EnhancedTouchSupport.Disable();
    }

    private void OnFingerDown(Finger finger)
    {
        Vector2 touchPosition = finger.screenPosition;
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("InfoObject"))
            {
                uiHUD.SetActive(false);
                uiInfo.transform.position = hit.collider.transform.position;
                uiInfo.transform.LookAt(arCamera);
                uiInfo.SetActive(true);

                Destroy(hit.collider.gameObject);
            }
        }
    }
}
