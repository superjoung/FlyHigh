using UnityEngine;
using UnityEngine.UI;

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

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        movement = Camera.main.transform.forward * moveZ + Camera.main.transform.right * moveX;
        movement.y = 0;
        movement.Normalize();
    }

    void MoveCharacter()
    {
        Vector3 movePosition = rb.position + movement * moveSpeed * Time.deltaTime;
        rb.MovePosition(movePosition);
    }

    void AnimateCharacter()
    {
        if (movement != Vector3.zero)
        {
            animator.SetTrigger("isWalk");
        }
        // 여기서는 "isWalk" 트리거가 활성화되면 애니메이션이 자동으로 초기화된다고 가정합니다.
        // "isWalk" 트리거를 수동으로 리셋해야 한다면 추가 코드가 필요합니다.
    }

    public DialogueManager dialogueManager;  // DialogueManager에 대한 참조

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            dialogueManager.StartDialogue("DialogueManager?");
        }
    }

}
