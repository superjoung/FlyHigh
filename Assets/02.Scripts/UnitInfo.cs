using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitInfo : MonoBehaviour
{
    public int characterID;
    public string characterNAME;
    public int dialogueGroupIndex;
    public GameObject unitPrefab; // 3D 모델
    public Sprite unitIcon;      // 인벤토리 내 아이콘
}
