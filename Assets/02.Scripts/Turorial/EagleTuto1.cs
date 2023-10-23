using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EagleTuto1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 movement;
    private Animator animator;
    CameraDialogue CD;
    //public DialogueManager dialogueManager;  // DialogueManager에 대한 참조
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        CD = GameObject.Find("Dialogue Camera").GetComponent<CameraDialogue>();
        //dialogueManager = GameObject.Find("manager").GetComponent<DialogueManager>();

        DialogueTutorial1 dt = FindObjectOfType<DialogueTutorial1>();
        dt.StartDialogue(0);
    }

    private void Update()
    {
        if (!DialogueManager.isInDialogue && !InventoryUI.isInventory)
        {
            ProcessInput();
            AnimateCharacter();

            if (!Input.GetMouseButton(0)) // 오른쪽 마우스 버튼을 누르지 않을 때만
            {
                RotateCharacterToCameraDirection();
            }
        }
        
    }

    void RotateCharacterToCameraDirection()
    {
        // 캐릭터가 카메라 방향을 바라보게 합니다.
        Vector3 lookDirection = new Vector3(movement.x, 0f, movement.z);
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void FixedUpdate()
    {
        if (!DialogueManager.isInDialogue && !InventoryUI.isInventory)
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

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            CD.enemyTransform = other.transform;
            UnitInfo character = other.GetComponent<UnitInfo>();
            //dialogueManager.StartDialogue(character);

            /*      */
            UnitManager unitManager = FindObjectOfType<UnitManager>();
            unitManager.AcquireUnit(character);
        }
    }
    

}
