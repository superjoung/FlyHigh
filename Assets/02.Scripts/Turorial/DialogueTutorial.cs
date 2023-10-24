using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueTutorial : MonoBehaviour
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
    public Text nameText;         // 대화 텍스트 말하는 캐릭터 이름
    public Button nextDialogueButton;
    public Camera mainCamera;         // 주 카메라의 참조
    public Camera dialogueCamera;     // 대화 뷰를 위한 카메라의 참조
    public static bool isInDialogue = false;  // 대화 중인지 나타내는 플래그
    private int originalMainCameraCullingMask;  // 주 카메라의 원래 cullingMask를 저장할 변수
    public static int dialogueCounter = 0;

    public AudioSource audioSource;     // AudioSource 컴포넌트에 대한 참조
    public AudioClip beepSound;         // "뾱" 소리에 대한 참조

    public Dictionary<int, DialogueSet> characterDialogues = new Dictionary<int, DialogueSet>();
    private Dictionary<int, int> currentDialogueIndices = new Dictionary<int, int>(); //몇번째 까지 했는지 출력
 
    private int currentCharacterID = -1;
    private int currentDialogueGroupIndex = -1;
    public static int idx;

    private void Start()
    {
        idx = 0;
        isInDialogue = true;
        // 시작 시 대화 박스 숨기기
        dialogueBox.SetActive(false);
        dialogueCamera.enabled = false;
        // 주 카메라의 원래 cullingMask를 저장합니다.
        originalMainCameraCullingMask = mainCamera.cullingMask;

        nextDialogueButton.onClick.AddListener(DisplayNextDialogue);

        characterDialogues[0] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "주민들",
                        "하늘아, 오늘도 순찰 잘 다녀와!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "네, 순찰 다녀오겠습니다.",
                        "오늘은 먹을 것도 많이 가져올게.",
                        "금방 다녀올테니까 다들 재밌게 놀고있어!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        " ",
                        "(하늘이가 떠난다)"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "주민들",
                        "하늘이는 참 대단하다니까.",
                        "우리 새들이 퍼덕 마을에서 안전하게 지낼 수 있는 건 하늘이가 호위대장인 덕분이라고 해도 과언이 아니지."}
                },
                new Dialogue{ dialogues = new List<string> {
                        "별이 (참새)",
                        "맞아. 게다가 하늘이는 참새잖아!",
                        "저 날개로 마을을 지키기위해 하늘이가 얼마나 노력했던지…"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "아리 (부엉이)",
                        "그러려면 먼저 밥부터 먹어야할 걸?",
                        "늦게오면 한 톨도 없어~~"}
                },
                new Dialogue{ dialogues = new List<string> {
                        " ",
                        "(아리가 먼저 간다)"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "주민들",
                        "같이가~!"}
                },

            }
        };

        characterDialogues[1] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "후우, 달이가 부탁했던 나뭇가지는 이정도면 되겠지?",
                        "이런, 벌써 시간이 이렇게 됐네.",
                        "애들이 많이 기다리겠다. 얼른 마을로 돌아가자."}
                }

            }
        };

    }

    private Coroutine typingCoroutine;

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";  // 대화 텍스트 초기화
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;  // 한 글자씩 텍스트에 추가
            if (!char.IsWhiteSpace(letter)) // 공백 문자가 아닐 경우에만 소리를 재생합니다.
            {
                audioSource.PlayOneShot(beepSound);  // "뾱" 소리 재생
            }
                
            yield return new WaitForSeconds(0.02f);  // 각 글자마다의 딜레이
        }
    }

    // 카메라가 바라보는 대상을 변경하는 메서드
    void ToggleCameraFocus()
    {
        Camtuto cameraDialogue = FindObjectOfType<Camtuto>();
        cameraDialogue.changeView(idx);
    }

    // 오디오 피치를 변경하는 메서드
    void ToggleAudioPitch()
    {

        audioSource.pitch = 3f;

        idx++;  // 대화 카운터 증가
    }

    public void StartDialogue(int character)
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

        currentCharacterID = character;
        //currentDialogueGroupIndex = character.dialogueGroupIndex;
        currentDialogueGroupIndex = 0;
        currentDialogueIndices[currentCharacterID] = 1;  // 대화 시작시 대화 인덱스를 0으로 초기화

        DisplayCurrentDialogue();
        ToggleCameraFocus();  // 카메라가 바라보는 대상 변경
        ToggleAudioPitch();   // 오디오 피치 변경
    }

    void DisplayCurrentDialogue()
    {
        Dialogue dialogue = characterDialogues[currentCharacterID].dialogueGroups[currentDialogueGroupIndex];

        nameText.text = dialogue.dialogues[0];

        if (currentDialogueIndices[currentCharacterID] < dialogue.dialogues.Count)
        {
            //dialogueText.text = dialogue.dialogues[currentDialogueIndices[currentCharacterID]];
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);  // 현재 타이핑 코루틴을 중지합니다.

            typingCoroutine = StartCoroutine(TypeSentence(dialogue.dialogues[currentDialogueIndices[currentCharacterID]]));
        }

        else
        {
            ToggleCameraFocus();  // 카메라가 바라보는 대상 변경
            ToggleAudioPitch();   // 오디오 피치 변경
            currentDialogueGroupIndex++;
            currentDialogueIndices[currentCharacterID] = 0;
            if (currentDialogueGroupIndex >= characterDialogues[currentCharacterID].dialogueGroups.Count)
                EndDialogue();
            else
                DisplayNextDialogue();
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
        //dialogueCamera.enabled = false;
        //isInDialogue = false;
        //mainCamera.cullingMask = originalMainCameraCullingMask;
    }
}