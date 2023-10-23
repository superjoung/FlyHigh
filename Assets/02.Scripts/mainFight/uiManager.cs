using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class uiManager : MonoBehaviour
{
    //--------------------------Control Panel------------------------------//
    public GameObject optionPanel;
    public GameObject SelectUnitPanel;

    public player Player;
    public playerUnitSet PlayerUnitSet;
    void Awake()
    {
        
    }
    private void Start()
    {
        Player = GameObject.Find("Player").GetComponent<player>();
        PlayerUnitSet = GameObject.Find("GameManger").GetComponent<playerUnitSet>();
    }

    public void OptionButtonClick()
    {
        optionPanel.SetActive(true);
    }
    public void ExitButtonClick()
    {
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;
        currentButton.transform.parent.gameObject.SetActive(false);
    }

    public void FightStart()
    {
        SelectUnitPanel.SetActive(false);
        PlayerUnitSet.isFightStart = true;

        for(int i = 0; i < PlayerUnitSet.ArrayNum; i++)
        {
            Player.palyerUnit[i] = GameObject.Find("UnitSetBox").transform.GetChild(i).gameObject;
        }
        GameObject.Find("UnitSetBox").SetActive(false);
    }
}
