using UnityEngine;

public class SmartFollower : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float idealDistance = 1.5f;
    public float tolerance = 0.3f; // 거리 오차 허용 범위

    private Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 targetFlatPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        float currentDistance = Vector3.Distance(transform.position, targetFlatPos);

        Vector3 direction = (targetFlatPos - transform.position).normalized;

        // 회전은 항상 유지
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // 이동 로직
        if (currentDistance > idealDistance + tolerance)
        {
            // 너무 멀다 → 따라가기
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else if (currentDistance < idealDistance - tolerance)
        {
            // 너무 가깝다 → 뒤로 가기
            transform.position -= direction * moveSpeed * Time.deltaTime;
        }
        // else: 적당한 거리 → 멈춤
    }
}
