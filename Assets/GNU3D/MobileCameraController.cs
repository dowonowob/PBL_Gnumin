using UnityEngine;

public class MobileCameraController : MonoBehaviour
{
    public Transform target;             // ĳ����(ginu)�� Transform
    public Vector2 sensitivity = new Vector2(2f, 2f);
    public Vector2 pitchClamp = new Vector2(-30f, 60f); // ���� ȸ�� ����
    public float distance = 5f;

    private float yaw = 0f;
    private float pitch = 20f; // �⺻������ ������ �ٶ󺸴� ����

    private Vector2 lastTouchPos;
    private bool isDragging;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("MobileCameraController: Target is not assigned.");
            return;
        }

        HandleTouchInput();

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 direction = rotation * Vector3.back;

        transform.position = target.position + direction * distance;
        transform.LookAt(target);
    }


    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 delta = touch.deltaPosition;

            if (touch.phase == TouchPhase.Moved)
            {
                yaw += delta.x * sensitivity.x * Time.deltaTime;
                pitch -= delta.y * sensitivity.y * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);
            }
        }
    }
}
