using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public GameObject[] enemyUnit = new GameObject[5];
    // ���� Text �߰� �� �̵� ����
    private void Start()
    {
        //FightEnemy enemy = GameObject.Find("FightEnemy").GetComponent<FightEnemy>();
        //enemyUnit = (GameObject[])enemy.enemyUnit.Clone();
    }
}
