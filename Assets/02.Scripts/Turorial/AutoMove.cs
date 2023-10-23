using UnityEngine;
using UnityEngine.AI;

public class AutoMove : MonoBehaviour
{
    public Transform[] waypoints; // 움직일 경로 지점들
    private int currentWaypointIndex = 0; // 현재 목표로 하는 경로 지점의 인덱스
    private NavMeshAgent agent;

    public static bool is_move1;
    public static bool is_move2;
    public static bool is_move3;

    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false; // 다음 경로 지점으로 자동으로 넘어갈 수 있게 설정
        is_move1 = false;
        is_move2 = false;
        is_move3 = false;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if ((is_move1 && gameObject.name == "Eagle") || (is_move2 && gameObject.name == "Owl") || is_move3)
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
