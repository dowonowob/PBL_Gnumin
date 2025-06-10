using UnityEngine;

public class MobilePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public float jumpForce = 5f;
    public FixedJoystick joystick;
    public Transform cameraTransform;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 input = joystick.Direction;
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * input.y + camRight * input.x;

        if (move.magnitude > 0.1f)
        {
            transform.Translate(move.normalized * moveSpeed * Time.deltaTime, Space.World);

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            if (animator != null)
            {
                animator.SetFloat("Speed", move.magnitude);
                animator.SetBool("isSitting", false);
            }
        }
        else
        {
            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }

        // 🔽 ESC(모바일 뒤로가기) 감지만 하고, 처리는 Android(Java) 쪽에서
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC 키(뒤로가기 버튼) 눌림 - Java onBackPressed()에서 처리함");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void TryJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = true;
        }
    }

    public void TriggerEat()
    {
        animator.SetTrigger("eat");
    }

    public void ToggleSit()
    {
        bool currentlySitting = animator.GetBool("isSitting");
        animator.SetBool("isSitting", !currentlySitting);
    }

    // 필요할 경우만 사용
    public void QuitApp()
    {
        Debug.Log("앱 종료 버튼 클릭됨");

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity != null)
                {
                    activity.Call("moveTaskToBack", true); // 앱을 백그라운드로 전환
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("안드로이드 종료 처리 중 오류: " + e.Message);
        }
#else
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
