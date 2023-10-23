using UnityEngine;
using UnityEngine.AI;

public class NavMove : MonoBehaviour
{
    public Transform[] waypoints; // 움직일 경로 지점들
    private int currentWaypointIndex = 0; // 현재 목표로 하는 경로 지점의 인덱스
    private NavMeshAgent agent;

    public bool is_move;
    public float speed = 5.0f;
    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false; // 다음 경로 지점으로 자동으로 넘어갈 수 있게 설정
        is_move = false;
        animator = GetComponent<Animator>();

        agent.speed = speed; // NavMeshAgent의 속도를 설정합니다.
    }

    private void Update()
    {
        if (is_move)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                MoveToNextWaypoint();
                animator.SetTrigger("isWalk");
            }
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) // 경로 지점이 설정되어 있지 않다면
            return;

        agent.destination = waypoints[currentWaypointIndex].position; // 현재 목표 경로 지점으로 설정
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // 다음 경로 지점의 인덱스로 변경
    }
}
