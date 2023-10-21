using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDialogue : MonoBehaviour
{
    public Transform playerTransform;
    public Transform enemyTransform;  // 대화하는 적의 Transform
    public float distanceFromPlayer = 15f;

    // Update is called once per frame
    void Update()
    {
        if (DialogueManager.isInDialogue)
        {
            SetDialogueCameraPosition();
        }
    }


    void SetDialogueCameraPosition()
    {
        Vector3 middlePoint = (playerTransform.position + enemyTransform.position) / 2;

        // 두 캐릭터 사이의 방향을 구합니다.
        Vector3 directionBetweenCharacters = (enemyTransform.position - playerTransform.position).normalized;

        // 두 캐릭터 사이의 방향과 수직인 방향을 구합니다.
        Vector3 perpendicularDirection = Vector3.Cross(directionBetweenCharacters, Vector3.up);

        // 카메라를 두 캐릭터 사이의 중앙 지점에서 해당 수직 방향으로 떨어뜨립니다. 거리를 더 늘리려면 `distanceFromPlayer` 값을 조절합니다.
        transform.position = middlePoint + perpendicularDirection * (distanceFromPlayer * 1.5f);  // 1.5배로 거리를 늘렸습니다.

        // 카메라의 높이를 높입니다.
        transform.position += Vector3.up * 5f;  // 5f만큼 높이를 높입니다. 원하는 높이에 따라 이 값을 조절하실 수 있습니다.

        // 카메라가 바라보는 지점을 조금 아래로 조정하여 15도 내려다보게 합니다.
        Vector3 lookAtTarget = middlePoint - Vector3.up * 2.5f;
        transform.LookAt(lookAtTarget);
    }

}
