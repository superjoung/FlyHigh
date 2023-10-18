using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    
    [System.Serializable]
    public class Dialogue
    {
        public List<string> dialogues = new List<string>();
    }

    [System.Serializable]
    public class DialogueSet
    {
        public List<Dialogue> dialogueGroups = new List<Dialogue>();
    }

    
    public GameObject dialogueBox;    // 대화 박스 (전체 UI 패널)
    public Text dialogueText;         // 대화 텍스트 UI에 대한 참조
    public Button nextDialogueButton;
    public Camera mainCamera;         // 주 카메라의 참조
    public Camera dialogueCamera;     // 대화 뷰를 위한 카메라의 참조
    public bool isInDialogue = false;  // 대화 중인지 나타내는 플래그
    private int originalMainCameraCullingMask;  // 주 카메라의 원래 cullingMask를 저장할 변수
    public Dictionary<int, DialogueSet> characterDialogues = new Dictionary<int, DialogueSet>();
    private Dictionary<int, int> currentDialogueIndices = new Dictionary<int, int>();

    private int currentCharacterID = -1;
    private int currentDialogueGroupIndex = -1;

    private void Start()
    {

        // 시작 시 대화 박스 숨기기
        dialogueBox.SetActive(false);
        dialogueCamera.enabled = false;
        // 주 카메라의 원래 cullingMask를 저장합니다.
        originalMainCameraCullingMask = mainCamera.cullingMask;

        nextDialogueButton.onClick.AddListener(DisplayNextDialogue);


        characterDialogues[1] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue
                {
                    dialogues = new List<string>
                    {
                        "처음 뵙는군요, 1번 적입니다.",
                        "또 만났네요!",
                        "이젠 익숙하죠?",
                        // 여기에 추가적인 대화를 계속 추가할 수 있습니다.
                    }
                },
                new Dialogue
                {
                    dialogues = new List<string>
                    {
                        "처dlldl, 1번 적입니다.",
                        "또 만dd네요!",
                        "이젠 dd죠?",
                        // 여기에 추가적인 대화를 계속 추가할 수 있습니다.
                    }
                }
            }
        };


    }


    public void StartDialogue(CharacterDialogue character)
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

        currentCharacterID = character.characterID;
        currentDialogueGroupIndex = character.dialogueGroupIndex;
        currentDialogueIndices[currentCharacterID] = 0;  // 대화 시작시 대화 인덱스를 0으로 초기화

        DisplayCurrentDialogue();
    }

    void DisplayCurrentDialogue()
    {
        Dialogue dialogue = characterDialogues[currentCharacterID].dialogueGroups[currentDialogueGroupIndex];

        if (currentDialogueIndices[currentCharacterID] < dialogue.dialogues.Count)
        {
            dialogueText.text = dialogue.dialogues[currentDialogueIndices[currentCharacterID]];
        }

        else
        {
            EndDialogue();
        }
    }

    void DisplayNextDialogue()
    {
        if (currentCharacterID == -1 || currentDialogueGroupIndex == -1) return;

        currentDialogueIndices[currentCharacterID]++;
        DisplayCurrentDialogue();
    }


    // 대화를 종료하는 메서드 (추후 다른 이벤트나 버튼에서 호출될 수 있음)
    public void EndDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        dialogueBox.SetActive(false);
        dialogueCamera.enabled = false;
        isInDialogue = false;
        mainCamera.cullingMask = originalMainCameraCullingMask;
    }
}



    


    

    

