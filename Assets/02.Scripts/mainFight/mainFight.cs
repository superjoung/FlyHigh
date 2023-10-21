using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Condition {
    public List<bool> isAbility     = new List<bool>();
    public List<bool> isHeat        = new List<bool>();
    public List<bool> isDead        = new List<bool>();
    public List<GameObject> animals = new List<GameObject>();
    public bool canUseAility;

    public Condition(bool _isAbility, bool _isHeat, bool _isDead, GameObject _animal) { 
        isAbility.Add(_isAbility); // 능력이 사용 가능한 상태인지
        isHeat.Add(_isHeat); // 공격을 받았을 때
        isDead.Add(_isDead); // 죽은 상태인지
        animals.Add(_animal); // 동일한 ID의 오브젝트가 들어왔을 때
        canUseAility = true;
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
    public GameObject[] playerUnit   = new GameObject[5]; // player 스크립트에서 unit 복사
    public GameObject[] enemyUnit    = new GameObject[5]; // enemy 스크립트에서 unit 복사
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
    public int playerUnitRemain;
    public int enemyUnitRemain;
    public int playerLastArray = 0;
    public int enemyLastArray  = 0;
    public float fightSpeed; // unit끼리 싸우는 속도를 조절. 값이 낮을 수록 빨리 전투합니다.
    public GameObject playerWinPanel;
    public GameObject enemyWinPanel;

    //-------------------------------Ability value--------------------------------//
    public int playerUseAbility  = 0; // player 능력 사용 카운트
    public int enemyUseAbility   = 0; // enemy 능력 사용 카운트
    public int playerAttackCount = 0; // player Attack 동일 한 유닛이 공격할 때만 카운트 증가
    public int enemyAttackCount  = 0; // enemy Attack 동일 한 유닛이 공격할 때만 카운트 증가
    public int playerDbuffTurn   = 0; // player can't use turn untill "playerDbuffTurn == 0"
    public int enemyDbuffTurn    = 0; // enemy can't use turn untill "playerDbuffTurn == 0"

    //---------------------------------Sound value--------------------------------//
    public AudioSource BGM_Sound;
    public AudioSource SFX_Sound;
    public AudioClip unitAttackSound;
    public AudioClip playerDefeatSound;
    public AudioClip playerWinSound;

    private void Start()
    {
        // 카메라 투영 방식을 2D 형식으로 변환
        Camera.main.orthographic = true;
        //------------------find correct obj into values----------------------//
        Enemy          = GameObject.Find("Enemy1").GetComponent<enemy>(); // 대화하는 대상 오브젝트의 enemy를 넣어주면 됩니다.
        Player         = GameObject.Find("Player").GetComponent<player>();

        playerBox      = GameObject.Find("PlayerAnimals");
        enemyBox       = GameObject.Find("EnemyAnimals");

        playerUiBox    = GameObject.Find("PlayerUiBox");
        enemyUiBox     = GameObject.Find("EnemyUiBox");

        playerUnit     = (GameObject[])Player.palyerUnit.Clone();
        enemyUnit      = (GameObject[])Enemy.enemyUnit.Clone();

        playerWinPanel = GameObject.Find("PlayerWinPanel");
        playerWinPanel.SetActive(false); // active -> false  / can't see this panel
        enemyWinPanel  = GameObject.Find("EnemyWinPanel");
        enemyWinPanel.SetActive(false); // active -> false  / can't see this panel

        // 1 ~ 6까지 Transform 불러와서 사용할 것. 0은 담고있는 부모오브젝트 저장
        playerSpawn    = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        enemySpawn     = GameObject.Find("EnemySpawnPoint").GetComponentsInChildren<Transform>();

        playerUiSpawn  = GameObject.Find("PlayerUiPosition").GetComponentsInChildren<RectTransform>();
        enemyUiSpawn   = GameObject.Find("EnemyUiPosition").GetComponentsInChildren<RectTransform>();

        // sound source 저장
        BGM_Sound = GameObject.Find("BGM_Manager").GetComponent<AudioSource>();
        SFX_Sound = GameObject.Find("SFX_Manager").GetComponent<AudioSource>();
        // ID 분리, dictionary에 저장
    }

    private void Update()
    {
        // 전투 시작
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnitAttackStart();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnUnit();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Ability Start");
            int a = playerUnitCount + playerUnitRemain;
            int b = enemyUnitCount + enemyUnitRemain;
            for (int i = playerUnitCount; i < a; i++)
            {
                if (unitCondition[playerUnitID[i]].isAbility[0])
                {
                    Debug.Log(playerUnitID[i]);
                    PlayerStartAbillty(playerUnitID[i]);
                }
            }
            for (int i = enemyUnitCount; i < b; i++)
            {
                if (unitCondition[enemyUnitID[i]].isAbility[0])
                {
                    Debug.Log(enemyUnitID[i]);
                    EnemyStartAbillty(enemyUnitID[i]);
                }
            }
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
                if (playerUnitID[unitCount] > 20 && playerUnitID[unitCount] <= 40)
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
                if (enemyUnitID[unitCount] > 1020 && enemyUnitID[unitCount] <= 1040)
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
                playerUnitRemain++;
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
                enemyUnitRemain++;            
            }
        }
    }

    void UnitAttackStart()
    {
        //--------------------전투 실시---------------------//
        // SFX_Sound에 유닛 공격 사운드 넣기
        SFX_Sound.clip = unitAttackSound;
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
        
        // 공격 사운드 넣어주기
        SFX_Sound.Play();
        // HP Text 접근 후 값 변경
        playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + playerID.Heart;
        enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + enmeyID.Heart;

        // player Unit이 기절했을 때
        if(playerID.Heart <= 0)
        {
            playerAttackCount = 0;
            playerCondition.isDead[0] = true;
            playerCondition.isHeat[0] = true;
            playerUnitRemain--;
            AbilityUseCheck(playerUnitID[playerUnitCount]);
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 2));
        }
        // player Unit이 기절을 안하고 타격 받았을 때
        else
        {
            enemyAttackCount++;
            playerCondition.isHeat[0] = true;
            AbilityUseCheck(playerUnitID[playerUnitCount]);
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 0)); // 0 player
        }

        // enemy Unit이 기절했을 때
        if(enmeyID.Heart <= 0)
        {
            enemyAttackCount = 0;
            enemyCondition.isDead[0] = true;
            enemyCondition.isHeat[0] = true;
            enemyUnitRemain--;
            AbilityUseCheck(enemyUnitID[enemyUnitCount]);
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 3));
        }
        // enemy Unit이 기절을 안하고 타격 받았을 때
        else
        {
            playerAttackCount++;
            enemyCondition.isHeat[0] = true;
            AbilityUseCheck(enemyUnitID[enemyUnitCount]);
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 1)); // 1 enemy
        }
        currentTurn++;
        Debug.Log("지금 턴은 : " + currentTurn);
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
        //------------------------------------------------//
        EndFightCheck();
    }

    // 사운드 및 Panel처리
    void EndFightCheck()
    {
        if(playerUnitRemain == 0) // player진형 패배 두 진형 동시에 기절했을 경우 패배로 처리
        {
            enemyWinPanel.SetActive(true);
            BGM_Sound.Stop();
            SFX_Sound.clip = playerDefeatSound;
            SFX_Sound.Play();
        }
        else if(enemyUnitRemain == 0) // enemy진형 패배
        {
            playerWinPanel.SetActive(true);
            BGM_Sound.Stop();
            SFX_Sound.clip = playerWinSound;
            SFX_Sound.Play();
        }
    }

    // Attack 실시할때마다 함수 실행
    void AbilityUseCheck(float ID)
    {   
        // attack animal field where detective
        // player
        if(ID > 0 && ID <= 100) // 현재 공격한 unit의 능력 조건 확인 가능
        {
            if(ID > 10 && ID <= 20 && unitCondition[ID].canUseAility == true)
            {
                unitCondition[ID].isAbility[0] = true;
                Debug.Log("Ability Set true : " + ID);
            }
            // 외부 unit들 능력 사용 필요 조건 ID 비교
            for (int current = 0; current < playerUnitID.Length; current++)
            {
                float currentID = playerUnitID[current];
                if (unitCondition[currentID].canUseAility == true)
                {
                    if (currentID > 0 && currentID <= 10 && playerUnitRemain == 1)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                    else if (currentID > 40 && currentID <= 50 && unitCondition[ID].isDead[0] == true)
                    {
                        if (currentID == 41 && (playerUnitCount == 2 || playerUnitCount == 4))
                        {
                            unitCondition[currentID].isAbility[0] = true;
                            Debug.Log("Ability Set true : " + currentID);
                        }
                        else if (currentID == 42 || currentID == 43)
                        {
                            unitCondition[currentID].isAbility[0] = true;
                            Debug.Log("Ability Set true : " + currentID);
                        }
                    }
                    else if (currentID > 50 && currentID <= 70 && unitCondition[ID].isHeat[0] == true)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                    else if (currentID > 80 && currentID <= 90 && enemyAttackCount == 3)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                }
            }
        }
        // enemy
        else if(ID > 1000 && ID <= 1100)
        {
            if (ID > 1010 && ID <= 1020 && unitCondition[ID].canUseAility == true)
            {
                unitCondition[ID].isAbility[0] = true;
                Debug.Log("Ability Set true : " + ID);
            }
            for (int current = 0; current < enemyUnitID.Length; current++)
            {
                float currentID = enemyUnitID[current];
                if (unitCondition[currentID].canUseAility == true)
                {
                    if (currentID > 1000 && currentID <= 1010 && enemyUnitRemain == 1)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                    else if (currentID > 1040 && currentID <= 1050 && unitCondition[ID].isDead[0] == true)
                    {
                        if (currentID == 1041 && (enemyUnitCount == 2 || enemyUnitCount == 4))
                        {
                            unitCondition[currentID].isAbility[0] = true;
                            Debug.Log("Ability Set true : " + currentID);
                        }
                        else if (currentID == 1042 || currentID == 1043)
                        {
                            unitCondition[currentID].isAbility[0] = true;
                            Debug.Log("Ability Set true : " + currentID);
                        }
                    }
                    else if (currentID > 1050 && currentID <= 1070 && unitCondition[ID].isHeat[0] == true)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                    else if (currentID > 1080 && currentID <= 1090 && playerAttackCount == 3)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                }
            }
        }
    }

    // 능력이 True가 됐을 때 실행해야함. case문 실행
    void PlayerStartAbillty(float ID)
    {
        GameObject currentAnimal = unitCondition[ID].animals[0];
        animalID temp;
        int CurrentNum = 0;
        int playerEndCount = playerUnitCount + playerUnitRemain; // playerUnitArray lastNum;
        int enemyEndCount = enemyUnitCount + enemyUnitRemain; // playerUnitArray lastNum;
        int inputNum = 0;
        int count = 0;

        for (int i = playerUnitCount; i < playerEndCount; i++)
        {
            if (playerUnitID[i] == ID) { CurrentNum = i; break; }
        }

        switch (ID)
        {
            case 1:
                currentAnimal.GetComponent<animalID>().Attack += 3;
                currentAnimal.GetComponent<animalID>().Heart += 3;
                playerUiBox.transform.GetChild(CurrentNum).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                playerUiBox.transform.GetChild(CurrentNum).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                unitCondition[ID].isAbility[0] = false;
                unitCondition[ID].canUseAility = false;
                break;

            case 11:
                if(unitCondition[enemyUnitID[enemyUnitCount]].isHeat[0])
                {
                    temp = unitCondition[enemyUnitID[enemyUnitCount]].animals[0].GetComponent<animalID>();
                    temp.Heart = 0;
                    enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                    unitCondition[enemyUnitID[enemyUnitCount]].isDead[0] = true;
                    UnitArraySet(enemyUnitCount, 1);
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 21:
                for(int i = 0; i < 2; i++)
                {
                    inputNum = Random.Range(playerUnitCount, playerEndCount);
                    inputNum = (playerUnitID[inputNum] != ID ? inputNum : (inputNum == playerEndCount - 1 ? --inputNum : ++inputNum));
                    temp = unitCondition[playerUnitID[inputNum]].animals[0].GetComponent<animalID>();
                    temp.Heart += 2;
                    playerUiBox.transform.GetChild(inputNum).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 22:
                while (true)
                {
                    if (unitCondition[playerUnitID[count]].animals[0] == currentAnimal)
                        break;
                    else ++count;
                }
                // 오브젝트가 처음에 있는지
                if(count == 0)
                {
                    temp = unitCondition[playerUnitID[count + 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    playerUiBox.transform.GetChild(count + 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                    playerUiBox.transform.GetChild(count + 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + temp.Attack;
                }
                // 오브젝트가 마지막에 있는지
                else if(count == playerEndCount)
                {
                    temp = unitCondition[playerUnitID[count - 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    playerUiBox.transform.GetChild(count - 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                    playerUiBox.transform.GetChild(count - 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + temp.Attack;
                }
                // 이상무! 양옆에 능력 부여
                else
                {
                    temp = unitCondition[playerUnitID[count - 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    playerUiBox.transform.GetChild(count - 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                    playerUiBox.transform.GetChild(count - 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + temp.Attack;

                    temp = unitCondition[playerUnitID[count + 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    playerUiBox.transform.GetChild(count + 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                    playerUiBox.transform.GetChild(count + 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + temp.Attack;
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 31:
                enemyDbuffTurn = 2;
                unitCondition[ID].isAbility[0] = false;
                break;

            case 41:
                for(int i = enemyUnitCount; i < enemyEndCount; i++)
                {
                    temp = unitCondition[enemyUnitID[i]].animals[0].GetComponent<animalID>();
                    temp.Heart -= 2;
                    enemyUiBox.transform.GetChild(i).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                    if (temp.Heart <= 0)
                    {
                        unitCondition[enemyUnitID[i]].isDead[0] = true;
                        UnitArraySet(i, 1);
                    }
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 42:
                inputNum = Random.Range(enemyUnitCount, enemyEndCount);
                unitCondition[enemyUnitID[inputNum]].canUseAility = false;
                unitCondition[ID].isAbility[0] = false;
                break;

            case 43:
                for(int i = enemyUnitCount; i < enemyEndCount; i++)
                {
                    temp = unitCondition[enemyUnitID[i]].animals[0].GetComponent<animalID>();
                    temp.Heart -= 1;
                    enemyUiBox.transform.GetChild(i).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                    if (temp.Heart <= 0)
                    {
                        unitCondition[enemyUnitID[i]].isDead[0] = true;
                        UnitArraySet(i, 1);
                    }
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 51:
                inputNum = Random.Range(enemyUnitCount, enemyEndCount);
                temp = unitCondition[enemyUnitID[inputNum]].animals[0].GetComponent<animalID>();
                temp.Heart -= 1;
                enemyUiBox.transform.GetChild(inputNum).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                if (temp.Heart <= 0)
                {
                    unitCondition[enemyUnitID[inputNum]].isDead[0] = true;
                    UnitArraySet(inputNum, 1);
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 61:
                if (unitCondition[playerUnitID[playerUnitCount]].isHeat[0])
                {
                    temp = unitCondition[playerUnitID[playerUnitCount]].animals[0].GetComponent<animalID>();
                    temp.Heart += 2;
                    playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 71:
                break;

            case 81:
                temp = unitCondition[enemyUnitID[enemyUnitCount]].animals[0].GetComponent<animalID>();
                temp.Heart -= 5;
                enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                if (temp.Heart <= 0)
                {
                    unitCondition[enemyUnitID[enemyUnitCount]].isDead[0] = true;
                    UnitArraySet(enemyUnitCount, 1);
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 82:
                break;

            default:
                Debug.LogError("not found anything Unit");
                break;
        }
    }

    void EnemyStartAbillty(float ID)
    {
        GameObject currentAnimal = unitCondition[ID].animals[0];
        animalID temp;
        int playerEndCount = playerUnitCount + playerUnitRemain;
        int enemyEndCount  = enemyUnitCount + enemyUnitRemain;
        int inputNum = 0;
        int count = 0;
        int CurrentNum = 0;

        for (int i = enemyUnitCount; i < enemyEndCount; i++)
        {
            if (enemyUnitID[i] == ID) { CurrentNum = i; break; }
        }

        switch ((int)ID)
        {
            case 1001: 
                currentAnimal.GetComponent<animalID>().Attack += 3;
                currentAnimal.GetComponent<animalID>().Heart += 3;
                enemyUiBox.transform.GetChild(CurrentNum).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                enemyUiBox.transform.GetChild(CurrentNum).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Attack;
                unitCondition[ID].isAbility[0] = false;
                unitCondition[ID].canUseAility = false;
                break;

            case 1011:
                if (unitCondition[playerUnitID[playerUnitCount]].isHeat[0])
                {
                    temp = unitCondition[playerUnitID[playerUnitCount]].animals[0].GetComponent<animalID>();
                    temp.Heart = 0;
                    playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                    unitCondition[playerUnitID[playerUnitCount]].isDead[0] = true;
                    UnitArraySet(playerUnitCount, 0);
                }
                unitCondition[ID].isAbility[0] = false;
                break;
            case 1021:
                for (int i = 0; i < 2; i++)
                {
                    inputNum = Random.Range(enemyUnitCount, enemyEndCount);
                    inputNum = (enemyUnitID[inputNum] != ID ? inputNum : (inputNum == enemyEndCount - 1 ? --inputNum : ++inputNum));
                    temp = unitCondition[enemyUnitID[inputNum]].animals[0].GetComponent<animalID>();
                    temp.Heart += 2;
                    enemyUiBox.transform.GetChild(inputNum).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1022:
                while (true)
                {
                    if (unitCondition[enemyUnitID[count]].animals[0] == currentAnimal)
                        break;
                    else ++count;
                }
                // 오브젝트가 처음에 있는지
                if (count == 0)
                {
                    temp = unitCondition[enemyUnitID[count + 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    enemyUiBox.transform.GetChild(count + 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                    enemyUiBox.transform.GetChild(count + 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Attack;
                }
                // 오브젝트가 마지막에 있는지
                else if (count == enemyEndCount)
                {
                    temp = unitCondition[enemyUnitID[count - 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    enemyUiBox.transform.GetChild(count - 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                    enemyUiBox.transform.GetChild(count - 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Attack;
                }
                // 이상무! 양옆에 능력 부여
                else
                {
                    temp = unitCondition[enemyUnitID[count - 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    enemyUiBox.transform.GetChild(count - 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                    enemyUiBox.transform.GetChild(count - 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Attack;

                    temp = unitCondition[enemyUnitID[count + 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    enemyUiBox.transform.GetChild(count + 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                    enemyUiBox.transform.GetChild(count + 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Attack;
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1031:
                playerDbuffTurn = 2;
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1041:
                for (int i = playerUnitCount; i < playerEndCount; i++)
                {
                    temp = unitCondition[playerUnitID[i]].animals[0].GetComponent<animalID>();
                    temp.Heart -= 2;
                    playerUiBox.transform.GetChild(i).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                    if (temp.Heart <= 0)
                    {
                        unitCondition[playerUnitID[i]].isDead[0] = true;
                        UnitArraySet(i, 0);
                    }
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1042:
                inputNum = Random.Range(playerUnitCount, playerEndCount);
                unitCondition[playerUnitID[inputNum]].canUseAility = false;
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1043:
                for (int i = playerUnitCount; i < playerEndCount; i++)
                {
                    temp = unitCondition[playerUnitID[i]].animals[0].GetComponent<animalID>();
                    temp.Heart -= 1;
                    playerUiBox.transform.GetChild(i).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                    if (temp.Heart <= 0)
                    {
                        unitCondition[playerUnitID[i]].isDead[0] = true;
                        UnitArraySet(i, 0);
                    }
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1051:
                inputNum = Random.Range(playerUnitCount, playerEndCount);
                temp = unitCondition[playerUnitID[inputNum]].animals[0].GetComponent<animalID>();
                temp.Heart -= 1;
                playerUiBox.transform.GetChild(inputNum).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                if (temp.Heart <= 0)
                {
                    unitCondition[playerUnitID[inputNum]].isDead[0] = true;
                    UnitArraySet(inputNum, 0);
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1061:
                if (unitCondition[enemyUnitID[enemyUnitCount]].isHeat[0])
                {
                    temp = unitCondition[enemyUnitID[enemyUnitCount]].animals[0].GetComponent<animalID>();
                    temp.Heart += 2;
                    enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1071:
                break;

            case 1081:
                temp = unitCondition[playerUnitID[playerUnitCount]].animals[0].GetComponent<animalID>();
                temp.Heart -= 5;
                playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;

                if (temp.Heart <= 0)
                {
                    unitCondition[playerUnitID[playerUnitCount]].isDead[0] = true;
                    UnitArraySet(playerUnitCount, 0);
                }
                unitCondition[ID].isAbility[0] = false;
                break;

            case 1082:
                break;

            default:
                Debug.LogError("not found anything Unit");
                break;
        }
    }

    // 중간에 배열이 죽었을 때 array 재조정 무조건 앞을 비우고 뒤부터 채운다.
    // 매개변수를 어떤걸로 채워야하나.. 유닛 카운트 번호로 넘기자!
    // 죽은 유닛 카운트입니다.
    void UnitArraySet(int unitCount, int checkNum) // checkNum 0 == player , 1 == enemy
    {
        Condition currentObj;

        if (checkNum == 0)
        {
            currentObj = unitCondition[playerUnitID[unitCount]];
            if (unitCount < playerUnitCount + playerUnitRemain - ++playerLastArray && unitCount > playerUnitCount) // playerUnitArrays 사이에 있을 경우
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                // 비어진 공간을 뒤에어부터 앞으로 채워주는 형식.
                for(int i = (unitCount + 1); i <= playerUnitCount + playerUnitRemain; i++)
                {
                    unitCondition[playerUnitID[i - 1]].animals[0].transform.DOMoveX(playerSpawn[i].position.x, fightSpeed).SetEase(Ease.OutSine);
                    playerUiBox.transform.GetChild(i - 1).GetComponent<RectTransform>().DOAnchorPosX(playerUiSpawn[i].position.x, fightSpeed).SetEase(Ease.OutSine);
                }

                for(int i =  unitCount; i < (playerUnitCount + playerUnitRemain - playerLastArray); i++)
                {
                    playerUnit[i] = playerUnit[i + 1];
                    playerUnitID[i] = playerUnitID[i + 1];
                    if (i == (playerUnitCount + playerUnitRemain - playerLastArray))
                    {
                        playerUnit[i + 1] = null;
                        playerUnitID[i + 1] = 0;
                    }
                }

                //--------isHeat & isDead 확인 후 능력 사용-------//
                if (unitCondition[playerUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --playerUnitRemain;
                }
            }
            else if (unitCount == playerUnitCount) // 앞에 있는 유닛이 사망했을 경우
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                if (unitCondition[playerUnitID[playerUnitCount]].isDead[0])
                {
                    playerBox.transform.DOMoveX(playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
                    playerUiBox.GetComponent<RectTransform>().DOAnchorPosX(75 * playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
                }
                //--------isHeat & isDead 확인 후 능력 사용-------//
                if (unitCondition[playerUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --playerUnitRemain;
                    ++playerUnitCount;
                    playerAttackCount = 0;
                }
            }
            else // 맨뒤에 있는 유닛이 죽었을경우
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));
                //--------isHeat & isDead 확인 후 능력 사용-------//
                if (unitCondition[playerUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --playerUnitRemain;
                }
            }
        }

        else if(checkNum == 1)
        {
            currentObj = unitCondition[enemyUnitID[unitCount]];
            if (unitCount < enemyUnitCount + enemyUnitRemain - (++enemyLastArray) && unitCount > enemyUnitCount) // playerUnitArrays 사이에 있을 경우
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                // 비어진 공간을 뒤에어부터 앞으로 채워주는 형식.
                for (int i = (unitCount + 1); i <= enemyUnitCount + enemyUnitRemain; i++)
                {
                    unitCondition[enemyUnitID[i - 1]].animals[0].transform.DOMoveX(enemySpawn[i].position.x, fightSpeed).SetEase(Ease.OutSine);
                    enemyUiBox.transform.GetChild(i - 1).GetComponent<RectTransform>().DOAnchorPosX(enemyUiSpawn[i].position.x, fightSpeed).SetEase(Ease.OutSine);
                }

                for (int i = unitCount; i < (enemyUnitCount + enemyUnitRemain - enemyLastArray); i++)
                {
                    enemyUnit[i] = enemyUnit[i + 1];
                    enemyUnitID[i] = enemyUnitID[i + 1];
                    if (i == (enemyUnitCount + enemyUnitRemain - enemyLastArray))
                    {
                        enemyUnit[i + 1] = null;
                        enemyUnitID[i + 1] = 0;
                    }
                }

                //--------isHeat & isDead 확인 후 능력 사용-------//
                if (unitCondition[enemyUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --enemyUnitRemain;
                }
            }
            else if (unitCount == enemyUnitCount) // 앞에 있는 유닛이 사망했을 경우
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                if (unitCondition[enemyUnitID[enemyUnitCount]].isDead[0])
                {
                    enemyBox.transform.DOMoveX(enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
                    enemyUiBox.GetComponent<RectTransform>().DOAnchorPosX(-75 * enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
                }
                //--------isHeat & isDead 확인 후 능력 사용-------//
                if (unitCondition[enemyUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --enemyUnitRemain;
                    ++enemyUnitCount;
                    enemyAttackCount = 0;
                }
            }
            else // 맨뒤에 있는 유닛이 죽었을경우
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));
                //--------isHeat & isDead 확인 후 능력 사용-------//
                if (unitCondition[enemyUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --enemyUnitRemain;
                }
            }
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

    IEnumerator StartDieAnim(GameObject animalObj, int checkNum, int unitCount) // checkNum 0 == player, 1 == enemy, 2 == playerDestroy, 3 == enemyDestroy
    {
        yield return new WaitForSeconds(0.01f); // 딜레이

        if (checkNum == 0)
        {
            animalObj.GetComponent<Animator>().SetTrigger("isDead");
            yield return new WaitForSeconds(0.01f);
            yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(animalObj);
            playerUiBox.transform.GetChild(unitCount).gameObject.SetActive(false);
        }
        else if (checkNum == 1)
        {
            animalObj.GetComponent<Animator>().SetTrigger("isDead");
            yield return new WaitForSeconds(0.01f);
            yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(animalObj);
            enemyUiBox.transform.GetChild(unitCount).gameObject.SetActive(false);
        }
    }
}