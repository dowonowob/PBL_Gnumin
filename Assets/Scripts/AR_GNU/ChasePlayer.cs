using UnityEngine;
using Niantic.Lightship.AR.NavigationMesh;
using UnityEngine.AI;
using System.Collections;

public class AgentChasePlayer : MonoBehaviour
{
    public static AgentChasePlayer Instance { get; private set; }

    public NavMeshAgent_Custom agent;
    public float updateInterval = 1.0f;
    public float followDistance = 1.5f;
    public float verticalOffset = -0.3f;

    public Animator animator;
    private Camera mainCamera;
    private float timer;
    private Transform chaseTarget = null;
    private bool isEating = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (agent == null || mainCamera == null || isEating)
            return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;

            float deviceSpeed = DeviceMotionTracker.Instance.Speed;
            float agentSpeed = Mathf.Clamp(deviceSpeed * 1.5f, 1.0f, 6.0f);
            agent.SetWalkingSpeed(agentSpeed);

            Vector3 targetPos = (chaseTarget != null)
                ? chaseTarget.position
                : mainCamera.transform.position + mainCamera.transform.forward * followDistance + mainCamera.transform.up * verticalOffset;

            agent.CustomAgentSetDestination(targetPos);
        }
    }

    public void SetTarget(Transform target)
    {
        chaseTarget = target;
    }

    public void ClearTarget()
    {
        chaseTarget = null;
    }

    public void PlayEatAnimation()
    {
        StartCoroutine(EatCoroutine());
    }

    private IEnumerator EatCoroutine()
    {
        isEating = true;
        agent.CustomAgentStopMoving();

        animator.SetTrigger("eatTrigger");

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("eat"));

        float eatDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(eatDuration);

        isEating = false;
    }

}