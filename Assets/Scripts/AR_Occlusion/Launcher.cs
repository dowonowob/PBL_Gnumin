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
        var cam = Camera.main;
        var pos = cam.transform.position;
        var forw = cam.transform.forward;

        var thing = Instantiate(_prefabWithRigidbody, pos + forw * 0.4f, Quaternion.identity);
        thing.AddForce(forw * 200.0f);
    }
}
