using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class Condition {
    private bool isAbility;
    private bool isHeat;
    private bool isDead;
    public List<GameObject> animals = new List<GameObject>();

    public Condition(bool _isAbility, bool _isHeat, bool _isDead, GameObject _animal) { 
        isAbility = _isAbility; // 능력이 사용 가능한 상태인지
        isHeat = _isHeat; // 공격을 받았을 때
        isDead = _isDead; // 죽은 상태인지
        animals.Add(_animal); // 동일한 ID의 오브젝트가 들어왔을 때
    }

    public void Show() {
        Debug.Log(isAbility);
        Debug.Log(isHeat);
        Debug.Log(isDead);
    }

    public GameObject UnitReturn()
    {
        return animals[0];
    }
}

public class mainFight : MonoBehaviour
{
    //------------------------Unit Condition & ID Input---------------------------//
    Dictionary <int, Condition> unitCondition = new Dictionary<int, Condition>();
    public player Player;
    public enemy Enemy;
    public GameObject[] playerUnit = new GameObject[5];
    public GameObject[] enemyUnit  = new GameObject[5];
    public int[] playerUnitID      = new int[5];
    public int[] enemyUnitID       = new int[5];

    //----------------------------Unit Spawn & UI---------------------------------//
    public Transform[] playerSpawn;
    public Transform[] enemySpawn;
    public RectTransform[] playerUiSpawn;
    public RectTransform[] enemyUiSpawn;
    public GameObject UnitConditionUi;

    //-------------------------Game Condition Value-------------------------------//
    public int currentTurn = 0;

    private void Start()
    {
        // 카메라 투영 방식을 2D 형식으로 변환
        Camera.main.orthographic = true;

        Enemy         = GameObject.Find("Enemy1").GetComponent<enemy>(); // 대화하는 대상 오브젝트의 enemy를 넣어주면 됩니다.
        Player        = GameObject.Find("Player").GetComponent<player>();

        playerUnit    = (GameObject[])Player.palyerUnit.Clone();
        enemyUnit     = (GameObject[])Enemy.enemyUnit.Clone();

        // 1 ~ 6까지 Transform 불러와서 사용할 것.
        playerSpawn   = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        enemySpawn    = GameObject.Find("EnemySpawnPoint").GetComponentsInChildren<Transform>();

        playerUiSpawn = GameObject.Find("PlayerUiPosition").GetComponentsInChildren<RectTransform>();
        enemyUiSpawn  = GameObject.Find("EnemyUiPosition").GetComponentsInChildren<RectTransform>();
        // ID 분리, dictionary에 저장
        SpawnUnit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnitAttackStart();
        }
    }

    // 유닛 아이디를 받아서 저장하는 역할
    void InputDictionary(int unitCount, GameObject addAnimal) {
        if (addAnimal.GetComponent<animalID>().UnitID < 1000)
        {
            playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID;

            // 초기 ID 값에 의거해 현재 상태를 넣어줌
            if (playerUnitID[unitCount] <= 10)
            {
                unitCondition.Add(playerUnitID[unitCount], new Condition(true, false, false, addAnimal));
            }
            else
            {
                unitCondition.Add(playerUnitID[unitCount], new Condition(false, false, false, addAnimal));
            }
        }

        else if (addAnimal.GetComponent<animalID>().UnitID > 1000)
        {
            enemyUnitID[unitCount] = enemyUnit[unitCount].GetComponent<animalID>().UnitID;

            if (enemyUnitID[unitCount] <= 1010)
            {
                unitCondition.Add(enemyUnitID[unitCount], new Condition(true, false, false, addAnimal));
                // unitCondition[enemyUnitID[unitCount]].Show(); // 값이 잘 들어가있는지 테스트
            }
            else
            {
                unitCondition.Add(enemyUnitID[unitCount], new Condition(false, false, false, addAnimal));
            }
        }
    }

    // 유닛 체력과 공격력 표시와 Unit 소환
    void SpawnUnit() { 
        // 아군, 적 unit 중 더 길이가 긴 배열 사이즈로 소환 시작
        for(int i = 0; i < (playerUnit.Length < enemyUnit.Length ? enemyUnit.Length : playerUnit.Length); i++)
        {
            if (playerUnit[i] != null) {
                GameObject addAnimal = Instantiate(playerUnit[i], playerSpawn[i + 1].position, Quaternion.Euler(0, 90, 0));
                InputDictionary(i, addAnimal);
                // UI Condition 소환
                GameObject setCondition = Instantiate(UnitConditionUi, playerUiSpawn[i + 1].transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + playerUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + playerUnit[i].GetComponent<animalID>().Attack;
            }

            if (enemyUnit[i] != null) {
                GameObject addAnimal = Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
                InputDictionary(i, addAnimal);
                // UI Condition 소환
                GameObject setCondition = Instantiate(UnitConditionUi, enemyUiSpawn[i + 1].transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + enemyUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + enemyUnit[i].GetComponent<animalID>().Attack;
            }
        }
    }

    void UnitAttackStart()
    {
        Debug.Log(unitCondition[enemyUnitID[0]].UnitReturn());
        unitCondition[enemyUnitID[0]].UnitReturn().GetComponent<Animator>().SetBool("isAttack", true);
        //unitCondition[enemyUnitID[0]].UnitReturn().GetComponent<Animator>().SetBool("isAttack", false);
    }
}
