using UnityEngine;
using UnityEngine.UI;  // Text를 사용하기 위해 필요한 네임스페이스

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;    // 대화 박스 (전체 UI 패널)
    public Text dialogueText;         // 대화 텍스트 UI에 대한 참조
    public Camera mainCamera;         // 주 카메라의 참조
    public Camera dialogueCamera;     // 대화 뷰를 위한 카메라의 참조

    private bool isInDialogue = false;  // 대화 중인지 확인하는 플래그
    private int originalMainCameraCullingMask;  // 주 카메라의 원래 cullingMask를 저장할 변수

    private void Start()
    {
        // 시작 시 대화 박스 숨기기
        dialogueBox.SetActive(false);
        dialogueCamera.enabled = false;
        // 주 카메라의 원래 cullingMask를 저장합니다.
        originalMainCameraCullingMask = mainCamera.cullingMask;
    }

    public void StartDialogue(string text)
    {
        if (!isInDialogue)
        {
            isInDialogue = true;

            // 마우스 커서 보이게 설정
            Cursor.lockState = CursorLockMode.None;

            // 대화 박스 표시
            dialogueBox.SetActive(true);

            // 주 카메라의 cullingMask를 조정하여 DialogueView만 렌더링하도록 합니다.
            mainCamera.cullingMask = LayerMask.GetMask("DialogueView");

            // 대화 카메라를 활성화하고 주 카메라의 cullingMask를 원래대로 돌려놓습니다.
            dialogueCamera.enabled = true;
            mainCamera.cullingMask = originalMainCameraCullingMask;

            // 주어진 텍스트를 표시합니다.
            dialogueText.text = text;
        }
    }

    // 대화를 종료하는 메서드 (추후 다른 이벤트나 버튼에서 호출될 수 있음)
    public void EndDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInDialogue = false;
        dialogueBox.SetActive(false);
        dialogueCamera.enabled = false;
        mainCamera.cullingMask = originalMainCameraCullingMask;
    }
}
