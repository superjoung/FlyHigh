using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Condition {
    private bool isAbility;
    private bool isHeat;
    private bool isDead;
    private List<GameObject> animals = new List<GameObject>();

    public Condition(bool _isAbility, bool _isHeat, bool _isDead, GameObject _animal) { 
        isAbility = _isAbility;
        isHeat = _isHeat;
        isDead = _isDead;
        animals.Add(_animal);
    }

    public void Show() {
        Debug.Log(isAbility);
        Debug.Log(isHeat);
        Debug.Log(isDead);
    }
}

public class mainFight : MonoBehaviour
{
    Dictionary <int, Condition> unitCondition = new Dictionary<int, Condition>();
    public player Player;
    public enemy Enemy;
    public GameObject[] playerUnit = new GameObject[5];
    public GameObject[] enemyUnit  = new GameObject[5];
    public int[] playerUnitID      = new int[5];
    public int[] enemyUnitID       = new int[5];

    private void Start()
    {
        Enemy = GameObject.Find("Enemy1").GetComponent<enemy>(); // 대화하는 대상 오브젝트의 enemy를 넣어주면 됩니다.
        Player = GameObject.Find("Player").GetComponent<player>();

        playerUnit = (GameObject[])Player.palyerUnit.Clone();
        enemyUnit = (GameObject[])Enemy.enemyUnit.Clone();

        // ID 분리, dictionary에 저장
        InputDictionary();
    }

    // 유닛 아이디를 받아서 저장하는 역할
    void InputDictionary() {
        for (int unitCount = 0; unitCount < 5; unitCount++)
        {
            if (playerUnit[unitCount] != null)
            {
                playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID;

                // 초기 ID 값에 의거해 현재 상태를 넣어줌
                if (playerUnitID[unitCount] <= 10)
                {
                    unitCondition.Add(playerUnitID[unitCount], new Condition(true, false, false, playerUnit[unitCount]));
                }
                else
                {
                    unitCondition.Add(playerUnitID[unitCount], new Condition(false, false, false, playerUnit[unitCount]));
                }
            }

            if (enemyUnit[unitCount] != null)
            {
                enemyUnitID[unitCount] = enemyUnit[unitCount].GetComponent<animalID>().UnitID;

                if (enemyUnitID[unitCount] <= 1010)
                {
                    unitCondition.Add(enemyUnitID[unitCount], new Condition(true, false, false, enemyUnit[unitCount]));
                    // unitCondition[enemyUnitID[unitCount]].Show(); // 값이 잘 들어가있는지 테스트
                }
                else
                {
                    unitCondition.Add(enemyUnitID[unitCount], new Condition(false, false, false, enemyUnit[unitCount]));
                }
            }
        }
    }
}
