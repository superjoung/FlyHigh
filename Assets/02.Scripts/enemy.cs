using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public GameObject[] enemyUnit = new GameObject[5];
    // 적군 Text 추가 및 이동 적용
    private void Start()
    {
        //FightEnemy enemy = GameObject.Find("FightEnemy").GetComponent<FightEnemy>();
        //enemyUnit = (GameObject[])enemy.enemyUnit.Clone();
    }
}
