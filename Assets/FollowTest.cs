using UnityEngine;

public class SmartFollower : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float idealDistance = 1.5f;
    public float tolerance = 0.3f; // �Ÿ� ���� ��� ����

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

        // ȸ���� �׻� ����
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // �̵� ����
        if (currentDistance > idealDistance + tolerance)
        {
            // �ʹ� �ִ� �� ���󰡱�
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else if (currentDistance < idealDistance - tolerance)
        {
            // �ʹ� ������ �� �ڷ� ����
            transform.position -= direction * moveSpeed * Time.deltaTime;
        }
        // else: ������ �Ÿ� �� ����
    }
}
