using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereProjectile : MonoBehaviour
{
    private float firstY = 0.0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ObjManager.Instance.RegisterObj(transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //���� ����
        if (transform.position.y < firstY - 0.1f)
        {

        }
        // Agent�� �浹
        if (collision.gameObject.CompareTag("Player"))
        {
            ObjManager.Instance.RemoveObj(transform);

            // Eat �ִϸ��̼� ����
            AgentChasePlayer.Instance.PlayEatAnimation();

            Destroy(gameObject);
        }
    }
}
