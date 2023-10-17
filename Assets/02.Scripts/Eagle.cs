using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Eagle : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 movement;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ProcessInput();
        AnimateCharacter();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    void ProcessInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        movement = new Vector3(moveX, 0f, moveZ).normalized;
    }

    void MoveCharacter()
    {
        Vector3 movePosition = rb.position + movement * moveSpeed * Time.deltaTime;

        if (movement != Vector3.zero)
        {
            // 캐릭터의 방향을 움직이는 방향으로 변경합니다.
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f);
        }

        rb.MovePosition(movePosition);
    }

    void AnimateCharacter()
    {
        if (movement != Vector3.zero)
        {
            animator.SetTrigger("isWalk");
        }
        // 주의: 여기서는 "isWalk" 트리거가 활성화되면 애니메이션이 자동으로 초기화된다고 가정합니다.
        // 만약 "isWalk" 트리거를 수동으로 리셋해야 한다면 추가 코드가 필요합니다.
    }
}

