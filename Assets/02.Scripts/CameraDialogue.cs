using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDialogue : MonoBehaviour
{
    public Transform playerTransform;
    public Transform enemyTransform;
    public float distanceFromPlayer = 15f;
    private List<GameObject> hiddenObjects = new List<GameObject>();
    private BoxCollider visibilityCollider;

    private void Start()
    {
        playerTransform = GameObject.Find("Eagle").GetComponent<Transform>();
        changeView();

        // 콜라이더 생성 및 설정
        visibilityCollider = gameObject.AddComponent<BoxCollider>();
        visibilityCollider.isTrigger = true;
    }

    void Update()
    {
        if (DialogueManager.isInDialogue)
        {
            SetDialogueCameraPosition();
            changeView();
            AdjustVisibilityCollider();
        }
        else
        {
            RestoreHiddenObjects();
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
        //Vector3 lookAtTarget = middlePoint - Vector3.up * 2.5f;
        //transform.LookAt(lookAtTarget);
        
        // player와 enemy가 서로를 바라보도록 합니다.
        playerTransform.LookAt(enemyTransform.position);
        enemyTransform.LookAt(playerTransform.position);
    }

    public void changeView()
    {
        if (DialogueManager.dialogueCounter % 2 != 0)  // 짝수 대화일 경우 플레이어를 바라봄
        {
            transform.LookAt(playerTransform);
        }
        else  // 홀수 대화일 경우 적을 바라봄
        {
            transform.LookAt(enemyTransform);
            //Debug.Log("cam2");
        }

    }


    //void AdjustVisibilityCollider()
    //{
    //    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
    //    Vector3 center = (playerTransform.position - transform.position) / 2.0f;
    //    visibilityCollider.center = center;
    //    visibilityCollider.size = new Vector3(5f, 5f, distanceToPlayer); // 5f는 콜라이더의 폭과 높이입니다. 원하는 대로 조절할 수 있습니다.
    //}

    void AdjustVisibilityCollider()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        visibilityCollider.center = new Vector3(0,0,0); // 카메라와 플레이어 사이의 중앙 지점으로 설정
        visibilityCollider.size = new Vector3(10f, 10f, distanceToPlayer*2); // 5f는 콜라이더의 폭과 높이입니다. 원하는 대로 조절할 수 있습니다.
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerTransform.gameObject && other.gameObject != enemyTransform.gameObject)
        {
            Renderer rend = other.gameObject.GetComponent<Renderer>();
            if (rend != null)
            {
                hiddenObjects.Add(other.gameObject);
                Color color = rend.material.color;
                color.a = 0.3f; // 투명도 조절
                rend.material.color = color;
            }
        }
    }

    void RestoreHiddenObjects()
    {
        foreach (var obj in hiddenObjects)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Color color = rend.material.color;
                color.a = 1.0f; // 투명도 복원
                rend.material.color = color;
            }
        }

        hiddenObjects.Clear();
    }

}
