using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject unitButtonPrefab;  // 유닛 아이콘을 위한 버튼 프리팹
    public Transform unitButtonContainer; // 버튼들을 저장할 부모 오브젝트
    public Transform unit3DViewTransform; // 3D 유닛을 표시할 위치

    private GameObject current3DUnit; // 현재 표시되는 3D 유닛
    public UnitManager unitManager;

    private bool isRotating = false;
    public static bool isInventory = false;

    private void Start()
    {
        unitManager = FindObjectOfType<UnitManager>();
        inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !DialogueManager.isInDialogue)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

            if (inventoryPanel.activeInHierarchy)
                RefreshInventory();

            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                isInventory = false;
            }
                
        }

        if (inventoryPanel.activeInHierarchy)
        {

            if (Input.GetMouseButtonDown(0))
            {
                isRotating = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isRotating = false;
            }
            if (isRotating)
            {
                float rotationSpeed = 8.0f;
                float horizontal = Input.GetAxis("Mouse X");
                unit3DViewTransform.Rotate(0, -horizontal * rotationSpeed, 0);
            }
        }

        
    }
    

    public void RefreshInventory()
    {

        // 마우스 커서 보이게 설정
        Cursor.lockState = CursorLockMode.None;
        isInventory = true;

        // 기존의 유닛 버튼들 제거
        foreach (Transform child in unitButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 획득한 유닛들을 인벤토리에 추가
        foreach (UnitInfo unit in unitManager.acquiredUnits)
        {
            GameObject btn = Instantiate(unitButtonPrefab, unitButtonContainer);
            btn.GetComponentInChildren<UnityEngine.UI.Image>().sprite = unit.unitIcon;
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Show3DUnit(unit));
        }
    }

    public void Show3DUnit(UnitInfo unit)
    {
        if (current3DUnit != null)
        {
            Destroy(current3DUnit);
        }

        current3DUnit = Instantiate(unit.unitPrefab, unit3DViewTransform);
        current3DUnit.transform.localScale = new Vector3(300, 300, 300);  // 크기를 200배로 설정
    }

}

