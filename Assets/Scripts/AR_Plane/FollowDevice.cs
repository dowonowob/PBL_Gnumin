using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FollowCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;
    public float idealDistance = 1.2f;
    public float tolerance = 0.2f;
    public bool horizontalOnly = true;

    [Header("Ground Detection")]
    public LayerMask groundLayer;        // 바닥 메시 레이어
    public float groundCheckDistance = 2f;
    public float ySmoothSpeed = 10f;

    private Transform cameraTransform;
    private Animator animator;

    private void Start()
    {
        cameraTransform = Camera.main?.transform;
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
            Debug.LogWarning("Main camera not found!");
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = cameraTransform.position;

        if (horizontalOnly)
            targetPosition.y = currentPosition.y;

        Vector3 toTarget = (targetPosition - currentPosition);
        float distance = toTarget.magnitude;

        // 회전 처리
        if (toTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // 거리 비교 후 이동
        if (distance > idealDistance + tolerance)
        {
            Move(toTarget.normalized);
        }
        else if (distance < idealDistance - tolerance)
        {
            Move(-toTarget.normalized);
        }
        else
        {
            SetMoving(false);
        }

        FollowGround(); // 바닥 높이에 맞게 Y 위치 보정
    }

    private void Move(Vector3 direction)
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
        SetMoving(true);
    }

    private void SetMoving(bool isMoving)
    {
        if (animator != null)
            animator.SetBool("IsMoving", isMoving);
    }

    private void FollowGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            float targetY = hit.point.y;
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * ySmoothSpeed);
            transform.position = pos;
        }
    }
}
