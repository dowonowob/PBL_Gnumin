using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    public Rigidbody _prefabWithRigidbody;
    private MobileControls _controls;

    private void Awake()
    {
        _controls = new MobileControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Fire.Newaction.performed += OnFire;
    }

    private void OnDisable()
    {
        _controls.Fire.Newaction.performed -= OnFire;
        _controls.Disable();
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        Vector2 screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        var cam = Camera.main;
        var spawnPos = cam.transform.position + ray.direction * 0.4f;

        var thing = Instantiate(_prefabWithRigidbody, spawnPos, Quaternion.identity);
        thing.AddForce(ray.direction * 200.0f);

        // 캐릭터가 타겟을 따라가도록 설정
        var agent = FindObjectOfType<AgentChasePlayer>();
        if (agent != null)
        {
            agent.SetTarget(thing.transform);
        }
    }
}
