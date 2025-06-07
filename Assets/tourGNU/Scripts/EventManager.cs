using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject canvas;
    public GameObject placeObject;

    private TouchInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new TouchInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Touch.TouchPress.performed += OnTouch;
    }

    private void OnDisable()
    {
        _inputActions.Touch.TouchPress.performed -= OnTouch;
        _inputActions.Disable();
    }

    private void OnTouch(InputAction.CallbackContext context)
    {
        Vector2 screenPos = context.ReadValue<Vector2>();

        // UI 터치 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Touchscreen.current.primaryTouch.touchId.ReadValue()))
            return;

        Ray ray = arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Heart_Up") || hit.collider.transform.root.CompareTag("Heart_Up"))
            {
                Debug.Log("터치 성공!");

                canvas.SetActive(true);

                Vector3 targetPosition = hit.collider.transform.position + hit.collider.transform.forward * 0.5f;
                targetPosition.y = placeObject.transform.position.y + 1.0f;
                canvas.transform.position = targetPosition;
            }
        }
    }

    private void Update()
    {
        if (canvas.activeSelf)
        {
            Vector3 directionToCamera = arCamera.transform.position - canvas.transform.position;
            directionToCamera.y = 0;
            canvas.transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
        else
        {
            placeObject.SetActive(true);
        }
    }
}
