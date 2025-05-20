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
        //낙하 감지
        if (transform.position.y < firstY - 0.1f)
        {

        }
        // Agent와 충돌
        if (collision.gameObject.CompareTag("Player"))
        {
            ObjManager.Instance.RemoveObj(transform);

            // Eat 애니메이션 실행
            AgentChasePlayer.Instance.PlayEatAnimation();

            Destroy(gameObject);
        }
    }
}
