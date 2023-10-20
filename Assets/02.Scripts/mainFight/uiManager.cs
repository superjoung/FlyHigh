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

    void Awake()
    {
        
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
}
