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

        // ����� ��ġ�� ���
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            var touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
            {
                Debug.Log("UI ������ ��ġ�Ǿ� �߻� ��ҵ�");
                return;
            }
        }
        else
        {
            Debug.Log("�Է� ����");
            return;
        }

        // ���� �߻� ����
        var cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(screenPos);
        var spawnPos = cam.transform.position + ray.direction * 0.4f;

        var thing = Instantiate(_prefabWithRigidbody, spawnPos, Quaternion.identity);
        thing.AddForce(ray.direction * 200f);

        Debug.Log($"[�߻� ����] ��ġ: {spawnPos}, ���ӽð�: {heldDuration}��");
    }
}
