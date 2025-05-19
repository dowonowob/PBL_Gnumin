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
            Vector3 targetPos;

            if (chaseTarget != null)
            {
                targetPos = chaseTarget.position;
            }
            else
            {
                Vector3 forward = mainCamera.transform.forward;
                Vector3 down = mainCamera.transform.up * verticalOffset;
                targetPos = mainCamera.transform.position + forward * followDistance + down;
            }

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

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isEating = false;
    }
}