using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class Condition {
    public bool isAbility;
    public bool isHeat;
    public bool isDead;
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

    public bool isAbilityReturn()
    {
        return isAbility;
    }

    public bool isHeatReturn()
    {
        return isHeat;
    }

    public bool isDeadReturn()
    {
        return isDead;
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
    private GameObject playerBox; // player animals 부모 오브젝트
    private GameObject enemyBox; // enemy animals 부모 오브젝트
    private GameObject playerUiBox; // palyer ui 부모 오브젝트
    private GameObject enemyUiBox; // enemy ui 부모 오브젝트

    //-------------------------Game Condition Value-------------------------------//
    public int currentTurn     = 0;
    public int playerUnitCount = 0;
    public int enemyUnitCount  = 0;
    public float fightSpeed;

    private void Start()
    {
        // 카메라 투영 방식을 2D 형식으로 변환
        Camera.main.orthographic = true;

        Enemy         = GameObject.Find("Enemy1").GetComponent<enemy>(); // 대화하는 대상 오브젝트의 enemy를 넣어주면 됩니다.
        Player        = GameObject.Find("Player").GetComponent<player>();

        playerBox     = GameObject.Find("PlayerAnimals");
        enemyBox      = GameObject.Find("EnemyAnimals");

        playerUiBox   = GameObject.Find("PlayerUiBox");
        enemyUiBox    = GameObject.Find("EnemyUiBox");

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
                addAnimal.transform.parent = playerBox.transform;
                InputDictionary(i, addAnimal);
                // UI Condition 소환
                GameObject setCondition = Instantiate(UnitConditionUi, playerUiSpawn[i + 1].transform.position, Quaternion.identity, playerUiBox.transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + playerUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + playerUnit[i].GetComponent<animalID>().Attack;
            }

            if (enemyUnit[i] != null) {
                GameObject addAnimal = Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
                addAnimal.transform.parent = enemyBox.transform;
                InputDictionary(i, addAnimal);
                // UI Condition 소환
                GameObject setCondition = Instantiate(UnitConditionUi, enemyUiSpawn[i + 1].transform.position, Quaternion.identity, enemyUiBox.transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + enemyUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + enemyUnit[i].GetComponent<animalID>().Attack;
            }
        }
    }

    void UnitAttackStart()
    {
        Condition playerCondition = unitCondition[playerUnitID[playerUnitCount]];
        Condition enemyCondition = unitCondition[enemyUnitID[enemyUnitCount]];

        playerCondition.UnitReturn().transform.DOMoveX(-0.5f, fightSpeed).SetEase(Ease.OutQuint);
        enemyCondition.UnitReturn().transform.DOMoveX(0.5f, fightSpeed).SetEase(Ease.OutQuint);

        playerCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");
        enemyCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");

        animalID playerID = playerCondition.UnitReturn().GetComponent<animalID>();
        animalID enmeyID = enemyCondition.UnitReturn().GetComponent<animalID>();
        
        playerID.Heart -= enmeyID.Attack;
        enmeyID.Heart -= playerID.Attack;

        playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + playerID.Heart;
        enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + enmeyID.Heart;

        if(playerID.Heart <= 0)
        {
            Debug.Log("player Dead");
            playerCondition.isDead = true;
            playerCondition.isHeat = true;
            Debug.Log("player heat : " + playerCondition.isHeat);
            Debug.Log("player dead : " + playerCondition.isDead);
        }
        else
        {
            Debug.Log("player Heat");
            playerCondition.isHeat = true;
        }

        if(enmeyID.Heart <= 0)
        {
            Debug.Log("enemy Dead");
            enemyCondition.isDead = true;
            enemyCondition.isHeat = true;
        }
        else
        {
            Debug.Log("enemy Heat");
            enemyCondition.isHeat = true;
            Debug.Log("enemy heat : " + enemyCondition.isHeat);
            Debug.Log("enemy dead : " + enemyCondition.isDead);
        }

        StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 0));
        StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 1));
    }

    IEnumerator StartAttackAnim(GameObject animalObj, int checkNum) // checkNum 0 == player, 1 == enemy
    {
        yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        
        if(checkNum == 0)
        {
            animalObj.transform.DOMoveX(-1, fightSpeed).SetEase(Ease.OutQuint);
        }
        else
        {
            animalObj.transform.DOMoveX(1, fightSpeed).SetEase(Ease.OutQuint);
        }
    }
}
