using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Launcher : MonoBehaviour
{
    public Rigidbody _prefabWithRigidbody;
    private MobileControls _controls;

    private float _touchStartTime = -0.3f;

    private void Awake()
    {
        _controls = new MobileControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Fire.Newaction.started += OnFireStarted;
        _controls.Fire.Newaction.canceled += OnFireCanceled;
    }

    private void OnDisable()
    {
        _controls.Fire.Newaction.started -= OnFireStarted;
        _controls.Fire.Newaction.canceled -= OnFireCanceled;
        _controls.Disable();
    }

    private void OnFireStarted(InputAction.CallbackContext context)
    {
        _touchStartTime = Time.time;
    }

    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        float heldDuration = Time.time - _touchStartTime;

        if (heldDuration < 0.3f)
        {
            _touchStartTime = -0.3f;
            heldDuration = 0f;
            return;
        }

        Vector2 screenPos;

        // 모바일 터치일 경우
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            var touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
            {
                Debug.Log("UI 위에서 터치되어 발사 취소됨");
                return;
            }
        }
        else
        {
            Debug.Log("입력 없음");
            return;
        }

        // 실제 발사 로직
        var cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(screenPos);
        var spawnPos = cam.transform.position + ray.direction * 0.4f;

        var thing = Instantiate(_prefabWithRigidbody, spawnPos, Quaternion.identity);
        thing.AddForce(ray.direction * 200f);

        Debug.Log($"[발사 성공] 위치: {spawnPos}, 지속시간: {heldDuration}초");
    }
}
