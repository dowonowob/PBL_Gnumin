using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereProjectile : MonoBehaviour
{
    private int bounceCount = 0;
    private bool isFixed = false;
    private float firstY;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        firstY = transform.position.y;
        SphereManager.Instance.RegisterSphere(transform);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            bounceCount++;

            if (bounceCount >= 2)
                FixSphere();
        }

        // ���� ����
        if (transform.position.y < firstY - 0.1f)
        {
            FixSphere();
        }
        // Agent�� �浹
        if (collision.gameObject.CompareTag("Player"))
        {
            SphereManager.Instance.RemoveSphere(transform);

            // Eat �ִϸ��̼� ����
            AgentChasePlayer.Instance.PlayEatAnimation();

            Destroy(gameObject);
        }
    }

    private void FixSphere()
    {
        if (isFixed) return;
        isFixed = true;
    }
}
