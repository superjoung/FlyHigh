using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class playerUnitSet : MonoBehaviour
{
    /*
    public List<GameObject> unitInventory = new List<GameObject>();
    public InventoryUI inventory;
    public UnitManager UnitManager;
    */
    //----------------------------------raycast-----------------------------//
    public GameObject selectUnitPanel;
    public Vector3 mainCamPosition;
    public RaycastHit hit;
    public GameObject dragAnchor;

    //--------------------------------inven---------------------------------//
    public GameObject namheuk;
    public GameObject inven;
    public GameObject invenP;

    //-------------------------------unit save------------------------------//
    public List<UnitInfo> acquiredUnits = new List<UnitInfo>();
    public GameObject[] setUnitCustom = new GameObject[5];
    public GameObject setUnitBox;
    public FightFriend fightFriend;
    public int ArrayNum = 0;
    public int count = 0;

    public bool isFightStart = false;

    private void Awake()
    {
        /*
        inventory = GameObject.Find("manager").GetComponent<InventoryUI>();
        UnitManager = inventory.unitManager;
        */
        invenP = GameObject.Find("Content");
        inven = GameObject.Find("Inventory");
        selectUnitPanel = GameObject.Find("SelectUnitPanel");
        setUnitBox = GameObject.Find("UnitSetBox");
        namheuk = GameObject.Find("Test");
        fightFriend = GameObject.Find("FightFriend").GetComponent<FightFriend>();
    }

    private void Start()
    {
        for(int i = 0; i < fightFriend.playerUnit.Count; i++)
        {
            acquiredUnits.Add(fightFriend.playerUnit[i].GetComponent<UnitInfo>());
        }
        selectUnitPanel.SetActive(false);
        mainCamPosition = Camera.main.transform.position;
        InputInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isFightStart)
        {
            if(selectUnitPanel.activeSelf)
                selectUnitPanel.SetActive(false);
            else
                selectUnitPanel.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("mouse Enter");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                dragAnchor = hit.transform.gameObject;
                Debug.Log(hit.transform.name);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            bool findCheck = false;
            for (int i = 0; i < ArrayNum; i++)
            {
                if (dragAnchor.name == setUnitBox.transform.GetChild(i).name && !findCheck)
                {
                    Destroy(dragAnchor);
                    findCheck = true;
                }
                if (findCheck)
                {
                    setUnitBox.transform.GetChild(i).transform.position = new Vector3(setUnitBox.transform.position.x + (-1 * i+1), 0, 0);
                }
            }
            if (findCheck)
            {
                ArrayNum--;
            }
        }
    }

    void InputInventory()
    {
        for(int i = 0; i < acquiredUnits.Count; i++)
        {
            if(i % 2 == 0)
            {
                RectTransform invenPos = inven.GetComponent<RectTransform>();
                invenPos.anchoredPosition += new Vector2(-80, -80);
                GameObject temp = Instantiate(namheuk, invenPos.position, Quaternion.identity, inven.gameObject.transform.parent.transform);
                temp.GetComponent<Image>().sprite = acquiredUnits[i].unitIcon;
                temp.name = "button" + i;
            }
            else
            {
                RectTransform invenPos = inven.GetComponent<RectTransform>();
                invenPos.anchoredPosition += new Vector2(80, 0);
                GameObject temp = Instantiate(namheuk, invenPos.position, Quaternion.identity, inven.gameObject.transform.parent.transform);
                temp.GetComponent<Image>().sprite = acquiredUnits[i].unitIcon;
                temp.name = "button" + i;
            }
        }
        Destroy(inven);
    }

    public void InventoryUnitClick()
    {
        if (ArrayNum < 5)
        {
            GameObject currentClickObj = EventSystem.current.currentSelectedGameObject;
            count = 0;
            while (true)
            {
                if (invenP.transform.GetChild(count).gameObject == currentClickObj)
                    break;
                else
                    count++;
            }
            GameObject temp = Instantiate(acquiredUnits[count].unitPrefab, setUnitBox.transform.position + new Vector3(-1 * ArrayNum, 0, 0), Quaternion.Euler(0, 90, 0), setUnitBox.transform);
            temp.transform.name = "obj" + ArrayNum;
            ArrayNum++;
        }
    }
}
