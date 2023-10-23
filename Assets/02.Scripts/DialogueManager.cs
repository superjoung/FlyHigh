using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

        characterDialogues[0] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "마을이… 어떻게 된거지? 왜 불타고 있는 거야?",
                        "저 곰들은… 곰곰 마을의 주민들이잖아! 왜 여기 있는 거지?"}
                }

            }
        };

        characterDialogues[1] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "곰곰마을 주민이 여기까지 무슨일이야!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "곰 잔당",
                        "마을은 불타고, 친구들이 납치되서야 나타나는 호위대장이라니..",
                        "참새 따위가 호위대장이니 마을이 이 모양 이 꼴이지!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "너희가 마을을 불태운 거야?",
                        "친구들을 어디로 데려간거야!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "곰 잔당",
                        "후후 우리가 납치했다!",
                        "생각해보면 우리 곰들이 가장 강한데 말이야, 너희들 따위랑 친구로 지낼 필요가 없잖아?",
                        "앞으로는 우리 곰곰 마을이 모든 마을을 지배할 것이라는 마왕곰님의 명령이 내려왔거든!",
                        "호위대장이면서 마을 하나 지킬 힘이 없다니, 참새 따위가 뭐 그렇지, 크앙!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "어디가는거야!",
                        "크윽 일단 남아있는 주민들 부터 찾아야겠어..."}
                }

            }
        };

        characterDialogues[2] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "친구들은 어디있지?",
                        "앗 구름아!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "구름이",
                        "미안해… 하늘아… 나… 아무것도 하지 못했어… ",
                        "곰들이 너무 무서워서… "}
                },
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "괜찮아 구름아. 중요한 건 앞으로 어떻게 하느냐잖아. ",
                        "우리 먼저 남은 친구들을 찾아보자."}
                },
                new Dialogue{ dialogues = new List<string> {
                        "구름이",
                        "저기…!",
                        "아리가 있어…!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "((구름이가 동료가 되었습니다.))" }
                }

            }
        };

        characterDialogues[3] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "아리야! 무사해서 다행이야!" }
                },
                new Dialogue{ dialogues = new List<string> {
                        "아리",
                        "저 앞에 곰 잔당들이 있어! 조심해"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "구름이",
                        "알겠어!",
                        "((아리가 동료가 되었습니다))"}
                },


            }
        };

        characterDialogues[4] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "우리 이대로 포기할 수 없잖아.",
                        "저 곰들은 우리 마을을 침략하느라 지금 많이 다쳤을 거야.",
                        "그래! 얼른 저 곰들 쫓아내버리러 가자!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        " ",
                        "앞에 곰 가까히 가면 전투가 시작됩니다."}
                }
            }
        };

        characterDialogues[5] = new DialogueSet
        {
            dialogueGroups = new List<Dialogue>
            {
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "헉 ! 물리쳤다!"}
                },
                new Dialogue{ dialogues = new List<string> {
                        "곰 잔당",
                        "크앙! 새들 주제에 아프잖아!",
                        "저 녀석은 참새 주제에 크기가 왜 저렇게 큰 거야!",
                        "크왕! 소용없는 짓이야.",
                        "어차피 다른 마을들도 다 끝났고… 곰들이 이곳으로 더 올거야.",
                        "너희가 할 수 있는 건 이제 없어."}
                },
                new Dialogue{ dialogues = new List<string> {
                        "하늘이",
                        "다른 마을들까지…",
                        "다른 마을들을 구하러 가자."}
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
        CameraDialogue cameraDialogue = FindObjectOfType<CameraDialogue>();
        cameraDialogue.changeView();
    }

    // 오디오 피치를 변경하는 메서드
    void ToggleAudioPitch()
    {
        if (dialogueCounter % 2 == 0)  // 짝수 대화일 경우 피치를 3으로
        {
            audioSource.pitch = 3f;
        }
        else  // 홀수 대화일 경우 피치를 1로
        {
            audioSource.pitch = 0.5f;
        }

        dialogueCounter++;  // 대화 카운터 증가
    }

    public void StartDialogue(DialogueInfo character)
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

        currentCharacterID = character.DialogueId;
        //currentDialogueGroupIndex = character.dialogueGroupIndex;
        currentDialogueGroupIndex = 0;
        dialogueCounter = 0;
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
        NavMove nm = Eagle.nowe.transform.root.gameObject.GetComponent<NavMove>();
        SphereCollider sc = Eagle.nowe.GetComponent<SphereCollider>();
        sc.radius = 0;
        nm.is_move = true;
        Cursor.lockState = CursorLockMode.Locked;
        dialogueBox.SetActive(false);
        dialogueCamera.enabled = false;
        isInDialogue = false;
        mainCamera.cullingMask = originalMainCameraCullingMask;
    }
}