using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ARMeshTapSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private LayerMask meshLayer; // "EnvironmentMesh" 같은 메싱 전용 레이어

    private List<GameObject> spawnedPrefabs = new();
    private AddCharacter _controls;

    private void Awake()
    {
        _controls = new AddCharacter();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Add.Newaction.performed += OnTap;
    }

    private void OnDisable()
    {
        _controls.Add.Newaction.performed -= OnTap;
        _controls.Disable();
    }

    private void OnTap(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, meshLayer))
        {
            Vector3 toCamera = Camera.main.transform.position - hit.point;
            toCamera.y = 0;

            if (toCamera.sqrMagnitude < 0.001f)
                toCamera = Vector3.forward;

            Quaternion lookRot = Quaternion.LookRotation(toCamera);

            var spawned = Instantiate(prefab, hit.point, lookRot);
            spawnedPrefabs.Add(spawned);
        }
    }
}
