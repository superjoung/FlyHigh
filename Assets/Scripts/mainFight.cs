using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition {
    public List<bool> isAbility     = new List<bool>();
    public List<bool> isHeat        = new List<bool>();
    public List<bool> isDead        = new List<bool>();
    public List<GameObject> animals = new List<GameObject>();

    public Condition(bool _isAbility, bool _isHeat, bool _isDead, GameObject _animal) { 
        isAbility.Add(_isAbility); // 능력이 사용 가능한 상태인지
        isHeat.Add(_isHeat); // 공격을 받았을 때
        isDead.Add(_isDead); // 죽은 상태인지
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
        return isAbility[0];
    }

    public bool isHeatReturn()
    {
        return isHeat[0];
    }

    public bool isDeadReturn()
    {
        return isDead[0];
    }
}

public class mainFight : MonoBehaviour
{
    //------------------------Unit Condition & ID Input---------------------------//
    Dictionary <float, Condition> unitCondition = new Dictionary<float, Condition>(); // 공격 받았는지, 능력사용 가능한지, 죽었는지, 해당 오브젝트를 Key값으로 찾아 사용
    public player Player;
    public enemy Enemy;
    public GameObject[] playerUnit = new GameObject[5]; // player 스크립트에서 unit 복사
    public GameObject[] enemyUnit  = new GameObject[5]; // enemy 스크립트에서 unit 복사
    public float[] playerUnitID      = new float[5]; // player unit id 에서 id만 뽑아서 배열 저장
    public float[] enemyUnitID       = new float[5]; // enemy unit id 에서 id만 뽑아서 배열 저장

    //----------------------------Unit Spawn & UI---------------------------------//
    public Transform[] playerSpawn; // player의 unit들을 소환하는 위치 값 저장
    public Transform[] enemySpawn; // enemy의 unit들을 소환하는 위치 값 저장
    public RectTransform[] playerUiSpawn; // player unit들의 condition을 나타내는 ui위치 저장
    public RectTransform[] enemyUiSpawn; // enemy unit들의 condition을 나타내는 ui위치 저장
    public GameObject UnitConditionUi; // AT와 HP 나타내는 Canvas UI
    private GameObject playerBox; // player animals 부모 오브젝트
    private GameObject enemyBox; // enemy animals 부모 오브젝트
    private GameObject playerUiBox; // palyer ui 부모 오브젝트
    private GameObject enemyUiBox; // enemy ui 부모 오브젝트

    //-------------------------Game Condition Value-------------------------------//
    public int currentTurn     = 0; // 진행한 turn 수, unit끼리 한번씩 부딪치면 1턴으로 간주한다.
    public int playerUnitCount = 0; // player unit들이 죽었을 경우 1씩 증가한다. 지금 뭘 움직여 싸워야하는지 구분할 때 사용
    public int enemyUnitCount  = 0; // enemy unit들이 죽었을 경우 1씩 증가한다. 지금 뭘 움직여 싸워야하는지 구분할 때 사용
    public float fightSpeed; // unit끼리 싸우는 속도를 조절. 값이 낮을 수록 빨리 전투합니다.

    private void Start()
    {
        // 카메라 투영 방식을 2D 형식으로 변환
        Camera.main.orthographic = true;
        //------------------find correct obj into values----------------------//
        Enemy         = GameObject.Find("Enemy1").GetComponent<enemy>(); // 대화하는 대상 오브젝트의 enemy를 넣어주면 됩니다.
        Player        = GameObject.Find("Player").GetComponent<player>();

        playerBox     = GameObject.Find("PlayerAnimals");
        enemyBox      = GameObject.Find("EnemyAnimals");

        playerUiBox   = GameObject.Find("PlayerUiBox");
        enemyUiBox    = GameObject.Find("EnemyUiBox");

        playerUnit    = (GameObject[])Player.palyerUnit.Clone();
        enemyUnit     = (GameObject[])Enemy.enemyUnit.Clone();

        // 1 ~ 6까지 Transform 불러와서 사용할 것. 0은 담고있는 부모오브젝트 저장
        playerSpawn   = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        enemySpawn    = GameObject.Find("EnemySpawnPoint").GetComponentsInChildren<Transform>();

        playerUiSpawn = GameObject.Find("PlayerUiPosition").GetComponentsInChildren<RectTransform>();
        enemyUiSpawn  = GameObject.Find("EnemyUiPosition").GetComponentsInChildren<RectTransform>();
        
        // ID 분리, dictionary에 저장
        SpawnUnit();
    }

    private void Update()
    {
        // 전투 시작
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnitAttackStart();
        }
    }

    // 유닛 아이디를 받아서 저장하는 역할
    // unitCount == 배열 index, addAnimal == 소환된 오브젝트, idValue == 동일 ID값의 오브젝트 접근시 기존ID + 0.1씩 증가
    void InputDictionary(int unitCount, GameObject addAnimal, float idValue) {
        if (addAnimal.GetComponent<animalID>().UnitID < 1000) // player unit 저장
        {
            playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID + idValue; // id 넘버 저장

            if (!(unitCondition.ContainsKey(playerUnitID[unitCount]))) // 현재 dictionary에 key값에 동일한 ID 값이 있는 지 확인 맞으면 True
            {
                // 초기 ID 값에 의거해 현재 상태를 넣어줌
                if (playerUnitID[unitCount] <= 10)
                {
                    // 전투 시작시 바로 능력이 사용되는 unit들 넣어주기
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(true, false, false, addAnimal));
                }
                else
                {
                    // 전투 시작시 바로 능력이 사용안되는 unit 넣어주기
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(false, false, false, addAnimal));
                }
            }
            else
            {
                // 중복일 경우 idValue 값 0.1증가 시켜주고 재귀
                InputDictionary(unitCount, addAnimal, idValue + 0.1f);
            }
        }
        // 위와 일맥상통함
        else if (addAnimal.GetComponent<animalID>().UnitID > 1000) // enemy unit 저장
        {
            enemyUnitID[unitCount] = enemyUnit[unitCount].GetComponent<animalID>().UnitID + idValue;

            if (!(unitCondition.ContainsKey(enemyUnitID[unitCount])))
            {
                if (enemyUnitID[unitCount] <= 1010)
                {
                    unitCondition.Add((enemyUnitID[unitCount]), new Condition(true, false, false, addAnimal));
                    // unitCondition[enemyUnitID[unitCount]].Show(); // 값이 잘 들어가있는지 테스트
                }
                else
                {
                    unitCondition.Add((enemyUnitID[unitCount]), new Condition(false, false, false, addAnimal));
                }
            }
            else
            {
                InputDictionary(unitCount, addAnimal, idValue + 0.1f);
            }
        }
    }

    // 유닛 체력과 공격력 표시와 Unit 소환
    void SpawnUnit() { 
        // 아군, 적 unit 중 더 길이가 긴 배열 사이즈로 소환 시작
        for(int i = 0; i < (playerUnit.Length < enemyUnit.Length ? enemyUnit.Length : playerUnit.Length); i++)
        {
            if (playerUnit[i] != null) {
                // prefabs를 하이레키창 오브젝트로 소환
                GameObject addAnimal = Instantiate(playerUnit[i], playerSpawn[i + 1].position, Quaternion.Euler(0, 90, 0));
                // playerBox 오브젝트안 자식으로 넣어줌
                addAnimal.transform.parent = playerBox.transform;
                InputDictionary(i, addAnimal, 0);
                // UI Condition 소환
                GameObject setCondition = Instantiate(UnitConditionUi, playerUiSpawn[i + 1].transform.position, Quaternion.identity, playerUiBox.transform);
                // text 설정 작업
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + playerUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + playerUnit[i].GetComponent<animalID>().Attack;
            }

            // 위 코드와 일맥상통.
            if (enemyUnit[i] != null) {
                GameObject addAnimal = Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
                addAnimal.transform.parent = enemyBox.transform;
                InputDictionary(i, addAnimal, 0);
                // UI Condition 소환
                GameObject setCondition = Instantiate(UnitConditionUi, enemyUiSpawn[i + 1].transform.position, Quaternion.identity, enemyUiBox.transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + enemyUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + enemyUnit[i].GetComponent<animalID>().Attack;
            }
        }
    }

    void UnitAttackStart()
    {
        //--------------------전투 실시---------------------//
        // dictionary에서 각 진형 앞열에 있는 unit의 Condition Class 가져오기
        Condition playerCondition = unitCondition[playerUnitID[playerUnitCount]];
        Condition enemyCondition = unitCondition[enemyUnitID[enemyUnitCount]];

        // Class에 담겨있는 오브젝트의 위치를 전투 위치로
        playerCondition.UnitReturn().transform.DOMoveX(-0.4f, fightSpeed).SetEase(Ease.OutQuint);
        enemyCondition.UnitReturn().transform.DOMoveX(0.4f, fightSpeed).SetEase(Ease.OutQuint);

        // Attack animator 실행 Trigger변수로 받아 한번만 실행
        playerCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");
        enemyCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");

        // animalID에서 고유한 AT, HT, ID 추출
        animalID playerID = playerCondition.UnitReturn().GetComponent<animalID>();
        animalID enmeyID = enemyCondition.UnitReturn().GetComponent<animalID>();
        
        // 상대 AT에서 HP를 빼줌
        playerID.Heart -= enmeyID.Attack;
        enmeyID.Heart -= playerID.Attack;

        // HP Text 접근 후 값 변경
        playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + playerID.Heart;
        enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + enmeyID.Heart;

        // player Unit이 기절했을 때
        if(playerID.Heart <= 0)
        {
            playerCondition.isDead[0] = true;
            playerCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 2));
        }
        // player Unit이 기절을 안하고 타격 받았을 때
        else
        {
            playerCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 0)); // 0 player
        }

        // enemy Unit이 기절했을 때
        if(enmeyID.Heart <= 0)
        {
            enemyCondition.isDead[0] = true;
            enemyCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 3));
        }
        // enemy Unit이 기절을 안하고 타격 받았을 때
        else
        {
            enemyCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 1)); // 1 enemy
        }
        //--------------------전투 종료---------------------//
    }

    void ControllConditionGame(int checkNum) // 0 player , 1 enemy
    {
        //------------컨디션 확인 후 결투 설정-------------//
        // 죽은 유닛 밀어내고 position 변경
        if (unitCondition[playerUnitID[playerUnitCount]].isDead[0] && checkNum == 0)
        {
            playerBox.transform.DOMoveX(playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
            playerUiBox.GetComponent<RectTransform>().DOAnchorPosX(75 * playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
        }

        if(unitCondition[enemyUnitID[enemyUnitCount]].isDead[0] && checkNum == 1)
        {
            enemyBox.transform.DOMoveX(-1*enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
            enemyUiBox.GetComponent<RectTransform>().DOAnchorPosX(-75 * enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
        }

        //--------isHeat & isDead 확인 후 능력 사용-------//
        //------------------------------------------------//
        if (unitCondition[playerUnitID[playerUnitCount]].isDead[0] && checkNum == 0)
        {
            unitCondition[playerUnitID[playerUnitCount]].isHeat.RemoveAt(0);
            unitCondition[playerUnitID[playerUnitCount]].isDead.RemoveAt(0);
            unitCondition[playerUnitID[playerUnitCount]].animals.RemoveAt(0);
            ++playerUnitCount;
        }

        if (unitCondition[enemyUnitID[enemyUnitCount]].isDead[0] && checkNum == 1)
        {
            unitCondition[enemyUnitID[enemyUnitCount]].isHeat.RemoveAt(0);
            unitCondition[enemyUnitID[enemyUnitCount]].isDead.RemoveAt(0);
            unitCondition[enemyUnitID[enemyUnitCount]].animals.RemoveAt(0);
            ++enemyUnitCount;
        }
        currentTurn++;
        EndFightCheck();
        Debug.Log("지금 턴은 : " + currentTurn);
    }

    // 사운드 및 Panel처리
    void EndFightCheck()
    {
        if(playerUnitCount == 5) // player진형 패배 두 진형 동시에 기절했을 경우 패배로 처리
        {

        }
        else if(enemyUnitCount == 5) // enemy진형 패배
        {

        }
    }

    IEnumerator StartAttackAnim(GameObject animalObj, int checkNum) // checkNum 0 == player, 1 == enemy, 2 == playerDestroy, 3 == enemyDestroy
    {
        yield return new WaitForSeconds(0.01f); // 딜레이
        yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        
        if(checkNum == 0)
        {
            animalObj.transform.DOMoveX(-1, fightSpeed).SetEase(Ease.OutQuint);
        }
        else if(checkNum == 1)
        {
            animalObj.transform.DOMoveX(1, fightSpeed).SetEase(Ease.OutQuint);
        }
        else if(checkNum == 2)
        {
            animalObj.GetComponent<Animator>().SetTrigger("isDead");
            yield return new WaitForSeconds(0.01f);
            yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(animalObj);
            playerUiBox.transform.GetChild(playerUnitCount).gameObject.SetActive(false);
            ControllConditionGame(0);
        }
        else if(checkNum == 3)
        {
            animalObj.GetComponent<Animator>().SetTrigger("isDead");
            yield return new WaitForSeconds(0.01f);
            yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(animalObj);
            enemyUiBox.transform.GetChild(enemyUnitCount).gameObject.SetActive(false);
            ControllConditionGame(1);
        }
    }
}
