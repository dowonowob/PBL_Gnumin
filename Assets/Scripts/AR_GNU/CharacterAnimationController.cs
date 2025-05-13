using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator _animator;

    public float idleDelay = 10f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetStand(bool value)
    {
        _animator.SetBool("isStand", value);
    }

    public void SetIdle(bool value)
    {
        _animator.SetBool("isIdle", value);
        if (value == true) _animator.SetBool("isStand", false);
    }
    public bool IsIdlePlaying()
    {
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName("idle_001");
    }



    public void SetWalking(bool value)
    {
        _animator.SetBool("isWalking", value);
        if (value == true)
        {
            _animator.SetBool("isStand", false);
            _animator.SetBool("isIdle", false);
        }
    }
}
