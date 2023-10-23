using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Camtuto1 : MonoBehaviour
{
    public Transform playerTransform;
    public Transform som;
    public Image fadePanel;  // 검은색 패널 참조
    public Image fadePanel2;
    private bool isFading = false;  // 페이딩 중인지 확인하는 플래그
    private bool isFading_ = false;  // 페이딩 중인지 확인하는 플래그
    public float distanceFromPlayer = 15f;
    public string nextSceneName = "pau_duck_burn_1"; // 다음 씬의 이름을 저장하는 변수
    private void Start()
    {
        isFading = false;
        isFading_ = true;
        playerTransform = GameObject.Find("Eagle").GetComponent<Transform>();
    }

    void Update()
    {
        if (DialogueTutorial1.isInDialogue)
        {
            SetDialogueCameraPosition();
            changeView(DialogueTutorial1.idx);
        }

        if (isFading)  // 페이딩 중이면
        {
            // 패널의 알파 값을 점점 증가시켜 검은색으로 만듭니다.
            Color currentColor = fadePanel2.color;
            currentColor.a += Time.deltaTime/2.0f;  // 값을 증가량 조절하여 페이딩 속도를 변경할 수 있습니다.
            fadePanel2.color = currentColor;

            if (currentColor.a >= 1f)  // 완전히 불투명해지면
            {
                isFading = false;  // 페이딩 중인 상태를 종료합니다.
                SceneManager.LoadScene(nextSceneName); // 다음 씬으로 전환
            }
        }


        if (isFading_)  // 페이딩 중이면
        {
            // 패널의 알파 값을 점점 증가시켜 검은색으로 만듭니다.
            Color currentColor = fadePanel.color;
            currentColor.a -= Time.deltaTime/2.0f;  // 값을 증가량 조절하여 페이딩 속도를 변경할 수 있습니다.
            fadePanel.color = currentColor;

            if (currentColor.a <= 0)  // 완전히 불투명해지면
            {
                isFading_ = false;  // 페이딩 중인 상태를 종료합니다.
            }
        }
    }

    void SetDialogueCameraPosition()
    {
        Vector3 middlePoint = (som.position + playerTransform.position) / 2;

        // 두 캐릭터 사이의 방향을 구합니다.
        Vector3 directionBetweenCharacters = (playerTransform.position - som.position).normalized;

        // 두 캐릭터 사이의 방향과 수직인 방향을 구합니다.
        Vector3 perpendicularDirection = Vector3.Cross(directionBetweenCharacters, Vector3.up);

        // 카메라를 두 캐릭터 사이의 중앙 지점에서 해당 수직 방향으로 떨어뜨립니다. 거리를 더 늘리려면 `distanceFromPlayer` 값을 조절합니다.
        transform.position = middlePoint + perpendicularDirection * (distanceFromPlayer * 0.8f);  // 1.5배로 거리를 늘렸습니다.

        // 카메라의 높이를 높입니다.
        transform.position += Vector3.up * 5f;  // 5f만큼 높이를 높입니다. 원하는 높이에 따라 이 값을 조절하실 수 있습니다.

        // 카메라가 바라보는 지점을 조금 아래로 조정하여 15도 내려다보게 합니다.
        Vector3 lookAtTarget = middlePoint - Vector3.up * 2.5f;
        transform.LookAt(lookAtTarget);     

    }

    public void changeView(int idx)
    {
        if (idx == 1)
        {
            AutoMove.is_move1 = true;
            StartCoroutine(DelayedFadeToBlack(3f));  // 3초 지연 후 페이딩을 시작합니다.
        }
    }

    IEnumerator DelayedFadeToBlack(float delay)
    {
        yield return new WaitForSeconds(delay);  // 지정된 지연 시간만큼 대기합니다.
        isFading = true;  // 페이딩을 시작합니다.
    }

}
