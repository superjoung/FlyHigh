using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Camtuto : MonoBehaviour
{
    public Transform playerTransform;
    public Transform sparrowTransform;
    public Transform sparrowTransform1;
    public Transform owlTransform;
    public Transform vTransform;
    public Transform vTransform1;

    public bool is_end;
    public Image fadePanel; // UI 패널 참조 추가
    public string nextSceneName = "pau_duck_1"; // 다음 씬의 이름을 저장하는 변수

    public float distanceFromPlayer = 15f;

    private void Start()
    {
        playerTransform = GameObject.Find("Eagle").GetComponent<Transform>();
        is_end = false;
    }

    void Update()
    {
        if (DialogueTutorial.isInDialogue)
        {
            SetDialogueCameraPosition();
            changeView(DialogueTutorial.idx);
        }
    }

    void SetDialogueCameraPosition()
    {
        Vector3 middlePoint = (sparrowTransform.position + vTransform1.position) / 2;

        // 두 캐릭터 사이의 방향을 구합니다.
        Vector3 directionBetweenCharacters = (-vTransform1.position + sparrowTransform.position).normalized;

        // 두 캐릭터 사이의 방향과 수직인 방향을 구합니다.
        Vector3 perpendicularDirection = Vector3.Cross(directionBetweenCharacters, Vector3.up);

        // 카메라를 두 캐릭터 사이의 중앙 지점에서 해당 수직 방향으로 떨어뜨립니다. 거리를 더 늘리려면 `distanceFromPlayer` 값을 조절합니다.
        transform.position = middlePoint + perpendicularDirection * (distanceFromPlayer * 0.8f);  // 1.5배로 거리를 늘렸습니다.

        // 카메라의 높이를 높입니다.
        transform.position += Vector3.up * 5f;  // 5f만큼 높이를 높입니다. 원하는 높이에 따라 이 값을 조절하실 수 있습니다.

        // 카메라가 바라보는 지점을 조금 아래로 조정하여 15도 내려다보게 합니다.
        //Vector3 lookAtTarget = middlePoint - Vector3.up * 2.5f;
        //transform.LookAt(lookAtTarget);

        // player와 enemy가 서로를 바라보도록 합니다.
        if (DialogueTutorial.idx < 2)
        {
            playerTransform.LookAt(vTransform.position);
            vTransform.LookAt(playerTransform.position);
            sparrowTransform.LookAt(vTransform1.position);
            vTransform1.LookAt(sparrowTransform.position);
            sparrowTransform1.LookAt(owlTransform.position);
            owlTransform.LookAt(sparrowTransform1.position);
        }
        

    }

    public void changeView(int idx)
    {
        idx--;
        Vector3 middlePoint = (sparrowTransform.position + vTransform1.position) / 2;
        Vector3 middlePoint1 = (vTransform1.position + vTransform.position) / 2;
        if (idx == 0 || idx == 3 || idx == 7) transform.LookAt(middlePoint1); //주민들 
        else if (idx == 1) transform.LookAt(playerTransform);
        else if (idx == 4) transform.LookAt(sparrowTransform1);
        else if (idx == 5) transform.LookAt(vTransform);

        if (idx == 2)
        {
            transform.LookAt(middlePoint);
            AutoMove.is_move1 = true;
        }


        if (idx == 6)
        {
            transform.LookAt(middlePoint);
            AutoMove.is_move2 = true;
        }

        if(idx == 7)
        {
            AutoMove.is_move3 = true;
            StartCoroutine(FadeAndLoadNextScene());
        }
    }


    IEnumerator FadeAndLoadNextScene()
    {
        yield return new WaitForSeconds(3f); // 3초 동안 대기

        float fadeDuration = 2f; // 1초 동안 화면이 어두워지게 설정
        float fadeSpeed = 1 / fadeDuration;
        float fadeAmount;
        is_end = true;
        // 화면이 점점 어두워지게 합니다.
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeAmount = t * fadeSpeed;
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, fadeAmount);
            yield return null;
        }

        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1); // 화면을 완전히 어둡게 합니다.

        SceneManager.LoadScene("pau_duck_1"); // 다음 씬으로 전환
    }

}
