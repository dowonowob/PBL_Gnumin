using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator _animator;
    private float _idleTimer = 0f;
    private bool _isIdleTimerRunning = false;

    public float idleDelay = 10f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Stand 상태에서 대기 타이머 작동
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("stand_001"))
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= idleDelay)
            {
                SetIdle(true);
                _idleTimer = 0f;
            }
        }
        else
        {
            _idleTimer = 0f;
        }
    }

    public void SetStand(bool value)
    {
        _animator.SetBool("isStand", value);
    }

    public void SetIdle(bool value)
    {
        _animator.SetBool("isIdle", value);
        if (value) _animator.SetBool("isStand", false);
    }

    public void SetWalking(bool value)
    {
        _animator.SetBool("isWalking", value);
        if (value)
        {
            _animator.SetBool("isStand", false);
            _animator.SetBool("isIdle", false);
        }
    }
}
