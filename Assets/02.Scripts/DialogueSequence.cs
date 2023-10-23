using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSequence : MonoBehaviour
{
    public GameObject haneul;  // 하늘이 캐릭터
    public GameObject ari;     // 아리 캐릭터
    public GameObject villagers;   // 퍼덕 마을 주민들

    public Text dialogueText;  // 대화 내용을 표시할 UI Text 오브젝트

    public AudioSource audioSource;     // AudioSource 컴포넌트에 대한 참조
    public AudioClip beepSound;         // "뾱" 소리에 대한 참조

    struct Dialogue
    {
        public GameObject speaker;
        public string text;

        public Dialogue(GameObject speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }
    }

    Queue<Dialogue> dialogues = new Queue<Dialogue>();

    void Start()
    {
        InitializeDialogues();
        StartCoroutine(PlayDialogue());
    }

    void InitializeDialogues()
    {
        dialogues.Enqueue(new Dialogue(villagers, "하늘아, 오늘도 순찰 잘 다녀와!"));
        dialogues.Enqueue(new Dialogue(haneul, "네, 순찰 다녀오겠습니다. 오늘은 먹을 것도 많이 가져올게. 금방 다녀올테니까 다들 재밌게 놀고있어!"));
        dialogues.Enqueue(new Dialogue(villagers, "하늘이는 참 대단하다니까. 우리 새들이 퍼덕 마을에서 안전하게 지낼 수 있는 건 하늘이가 호위대장인 덕분이라고 해도 과언이 아니지."));
        dialogues.Enqueue(new Dialogue(null, "별이(용감한 참새): 맞아. 게다가 하늘이는 참새잖아!  저 날개로 마을을 지키기위해 하늘이가 얼마나 노력했던지…"));
        dialogues.Enqueue(new Dialogue(null, "구름이(겁쟁이 참새): 나도… 나도 꼭 하늘이처럼 멋있는 호위대가 되어서 마을을 지킬 거야."));
        dialogues.Enqueue(new Dialogue(ari, "그러려면 먼저 밥부터 먹어야할 걸? 늦게오면 한 톨도 없어~~"));
        dialogues.Enqueue(new Dialogue(villagers, "야, 같이가!!!"));
        dialogues.Enqueue(new Dialogue(haneul, "후우, 달이가 부탁했던 나뭇가지는 이정도면 되겠지?\n이런, 벌써 시간이 이렇게 됐네. 애들이 많이 기다리겠다. 얼른 마을로 돌아가자."));
    }

    IEnumerator PlayDialogue()
    {
        while (dialogues.Count > 0)
        {
            Dialogue currentDialogue = dialogues.Dequeue();

            // 여기를 수정합니다. 직접 텍스트를 할당하는 대신 TypeSentence 코루틴을 시작합니다.
            if (typingCoroutine != null) // 이미 진행 중인 TypeSentence 코루틴이 있다면 중지합니다.
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeSentence(currentDialogue.text));

            yield return new WaitUntil(() => dialogueText.text == currentDialogue.text); // 문장이 완전히 출력될 때까지 대기합니다.
            yield return new WaitForSeconds(3);  // 각 대사마다 3초 대기. 필요에 따라 조절 가능
        }

        // 모든 대사가 끝난 후, 예를 들어 하늘이 움직이는 코드
        haneul.transform.position += new Vector3(5, 0, 0);  // 원하는 방향과 거리로 이동
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

    

}
