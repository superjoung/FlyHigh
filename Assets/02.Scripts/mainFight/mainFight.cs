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
        isAbility.Add(_isAbility); // �ɷ��� ��� ������ ��������
        isHeat.Add(_isHeat); // ������ �޾��� ��
        isDead.Add(_isDead); // ���� ��������
        animals.Add(_animal); // ������ ID�� ������Ʈ�� ������ ��
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
    Dictionary <float, Condition> unitCondition = new Dictionary<float, Condition>(); // ���� �޾Ҵ���, �ɷ»�� ��������, �׾�����, �ش� ������Ʈ�� Key������ ã�� ���
    public player Player;
    public enemy Enemy;
    public GameObject[] playerUnit   = new GameObject[5]; // player ��ũ��Ʈ���� unit ����
    public GameObject[] enemyUnit    = new GameObject[5]; // enemy ��ũ��Ʈ���� unit ����
    public float[] playerUnitID      = new float[5]; // player unit id ���� id�� �̾Ƽ� �迭 ����
    public float[] enemyUnitID       = new float[5]; // enemy unit id ���� id�� �̾Ƽ� �迭 ����

    //----------------------------Unit Spawn & UI---------------------------------//
    public Transform[] playerSpawn; // player�� unit���� ��ȯ�ϴ� ��ġ �� ����
    public Transform[] enemySpawn; // enemy�� unit���� ��ȯ�ϴ� ��ġ �� ����
    public RectTransform[] playerUiSpawn; // player unit���� condition�� ��Ÿ���� ui��ġ ����
    public RectTransform[] enemyUiSpawn; // enemy unit���� condition�� ��Ÿ���� ui��ġ ����
    public GameObject UnitConditionUi; // AT�� HP ��Ÿ���� Canvas UI
    private GameObject playerBox; // player animals �θ� ������Ʈ
    private GameObject enemyBox; // enemy animals �θ� ������Ʈ
    private GameObject playerUiBox; // palyer ui �θ� ������Ʈ
    private GameObject enemyUiBox; // enemy ui �θ� ������Ʈ

    //-------------------------Game Condition Value-------------------------------//
    public int currentTurn     = 0; // ������ turn ��, unit���� �ѹ��� �ε�ġ�� 1������ �����Ѵ�.
    public int playerUnitCount = 0; // player unit���� �׾��� ��� 1�� �����Ѵ�. ���� �� ������ �ο����ϴ��� ������ �� ���
    public int enemyUnitCount  = 0; // enemy unit���� �׾��� ��� 1�� �����Ѵ�. ���� �� ������ �ο����ϴ��� ������ �� ���
    public int playerUnitRemain;
    public int enemyUnitRemain;
    public int playerLastArray = 0;
    public int enemyLastArray  = 0;
    public float fightSpeed; // unit���� �ο�� �ӵ��� ����. ���� ���� ���� ���� �����մϴ�.
    public GameObject playerWinPanel;
    public GameObject enemyWinPanel;

    //-------------------------------Ability value--------------------------------//
    public int playerUseAbility  = 0; // player �ɷ� ��� ī��Ʈ
    public int enemyUseAbility   = 0; // enemy �ɷ� ��� ī��Ʈ
    public int playerAttackCount = 0; // player Attack ���� �� ������ ������ ���� ī��Ʈ ����
    public int enemyAttackCount  = 0; // enemy Attack ���� �� ������ ������ ���� ī��Ʈ ����
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
        // ī�޶� ���� ����� 2D �������� ��ȯ
        Camera.main.orthographic = true;
        //------------------find correct obj into values----------------------//
        Enemy          = GameObject.Find("Enemy1").GetComponent<enemy>(); // ��ȭ�ϴ� ��� ������Ʈ�� enemy�� �־��ָ� �˴ϴ�.
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

        // 1 ~ 6���� Transform �ҷ��ͼ� ����� ��. 0�� ����ִ� �θ������Ʈ ����
        playerSpawn    = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        enemySpawn     = GameObject.Find("EnemySpawnPoint").GetComponentsInChildren<Transform>();

        playerUiSpawn  = GameObject.Find("PlayerUiPosition").GetComponentsInChildren<RectTransform>();
        enemyUiSpawn   = GameObject.Find("EnemyUiPosition").GetComponentsInChildren<RectTransform>();

        // sound source ����
        BGM_Sound = GameObject.Find("BGM_Manager").GetComponent<AudioSource>();
        SFX_Sound = GameObject.Find("SFX_Manager").GetComponent<AudioSource>();
        // ID �и�, dictionary�� ����
    }

    private void Update()
    {
        // ���� ����
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

    // ���� ���̵� �޾Ƽ� �����ϴ� ����
    // unitCount == �迭 index, addAnimal == ��ȯ�� ������Ʈ, idValue == ���� ID���� ������Ʈ ���ٽ� ����ID + 0.1�� ����
    void InputDictionary(int unitCount, GameObject addAnimal, float idValue) {
        if (addAnimal.GetComponent<animalID>().UnitID < 1000) // player unit ����
        {
            playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID + idValue; // id �ѹ� ����

            if (!(unitCondition.ContainsKey(playerUnitID[unitCount]))) // ���� dictionary�� key���� ������ ID ���� �ִ� �� Ȯ�� ������ True
            {
                // �ʱ� ID ���� �ǰ��� ���� ���¸� �־���
                if (playerUnitID[unitCount] > 20 && playerUnitID[unitCount] <= 40)
                {
                    // ���� ���۽� �ٷ� �ɷ��� ���Ǵ� unit�� �־��ֱ�
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(true, false, false, addAnimal));
                }
                else
                {
                    // ���� ���۽� �ٷ� �ɷ��� ���ȵǴ� unit �־��ֱ�
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(false, false, false, addAnimal));
                }
            }
            else
            {
                // �ߺ��� ��� idValue �� 0.1���� �����ְ� ���
                InputDictionary(unitCount, addAnimal, idValue + 0.1f);
            }
        }
        // ���� �ϸƻ�����
        else if (addAnimal.GetComponent<animalID>().UnitID > 1000) // enemy unit ����
        {
            enemyUnitID[unitCount] = enemyUnit[unitCount].GetComponent<animalID>().UnitID + idValue;

            if (!(unitCondition.ContainsKey(enemyUnitID[unitCount])))
            {
                if (enemyUnitID[unitCount] > 1020 && enemyUnitID[unitCount] <= 1040)
                {
                    unitCondition.Add((enemyUnitID[unitCount]), new Condition(true, false, false, addAnimal));
                    // unitCondition[enemyUnitID[unitCount]].Show(); // ���� �� ���ִ��� �׽�Ʈ
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

    // ���� ü�°� ���ݷ� ǥ�ÿ� Unit ��ȯ
    void SpawnUnit() { 
        // �Ʊ�, �� unit �� �� ���̰� �� �迭 ������� ��ȯ ����
        for(int i = 0; i < (playerUnit.Length < enemyUnit.Length ? enemyUnit.Length : playerUnit.Length); i++)
        {
            if (playerUnit[i] != null) {
                // prefabs�� ���̷�Űâ ������Ʈ�� ��ȯ
                GameObject addAnimal = Instantiate(playerUnit[i], playerSpawn[i + 1].position, Quaternion.Euler(0, 90, 0));
                // playerBox ������Ʈ�� �ڽ����� �־���
                addAnimal.transform.parent = playerBox.transform;
                InputDictionary(i, addAnimal, 0);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, playerUiSpawn[i + 1].transform.position, Quaternion.identity, playerUiBox.transform);
                // text ���� �۾�
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + playerUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + playerUnit[i].GetComponent<animalID>().Attack;
                playerUnitRemain++;
            }

            // �� �ڵ�� �ϸƻ���.
            if (enemyUnit[i] != null) {
                GameObject addAnimal = Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
                addAnimal.transform.parent = enemyBox.transform;
                InputDictionary(i, addAnimal, 0);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, enemyUiSpawn[i + 1].transform.position, Quaternion.identity, enemyUiBox.transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + enemyUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + enemyUnit[i].GetComponent<animalID>().Attack;
                enemyUnitRemain++;            
            }
        }
    }

    void UnitAttackStart()
    {
        //--------------------���� �ǽ�---------------------//
        // SFX_Sound�� ���� ���� ���� �ֱ�
        SFX_Sound.clip = unitAttackSound;
        // dictionary���� �� ���� �տ��� �ִ� unit�� Condition Class ��������
        Condition playerCondition = unitCondition[playerUnitID[playerUnitCount]];
        Condition enemyCondition = unitCondition[enemyUnitID[enemyUnitCount]];

        // Class�� ����ִ� ������Ʈ�� ��ġ�� ���� ��ġ��
        playerCondition.UnitReturn().transform.DOMoveX(-0.4f, fightSpeed).SetEase(Ease.OutQuint);
        enemyCondition.UnitReturn().transform.DOMoveX(0.4f, fightSpeed).SetEase(Ease.OutQuint);

        // Attack animator ���� Trigger������ �޾� �ѹ��� ����
        playerCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");
        enemyCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");

        // animalID���� ������ AT, HT, ID ����
        animalID playerID = playerCondition.UnitReturn().GetComponent<animalID>();
        animalID enmeyID = enemyCondition.UnitReturn().GetComponent<animalID>();
        
        // ��� AT���� HP�� ����
        playerID.Heart -= enmeyID.Attack;
        enmeyID.Heart -= playerID.Attack;
        
        // ���� ���� �־��ֱ�
        SFX_Sound.Play();
        // HP Text ���� �� �� ����
        playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + playerID.Heart;
        enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + enmeyID.Heart;

        // player Unit�� �������� ��
        if(playerID.Heart <= 0)
        {
            playerAttackCount = 0;
            playerCondition.isDead[0] = true;
            playerCondition.isHeat[0] = true;
            playerUnitRemain--;
            AbilityUseCheck(playerUnitID[playerUnitCount]);
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 2));
        }
        // player Unit�� ������ ���ϰ� Ÿ�� �޾��� ��
        else
        {
            enemyAttackCount++;
            playerCondition.isHeat[0] = true;
            AbilityUseCheck(playerUnitID[playerUnitCount]);
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 0)); // 0 player
        }

        // enemy Unit�� �������� ��
        if(enmeyID.Heart <= 0)
        {
            enemyAttackCount = 0;
            enemyCondition.isDead[0] = true;
            enemyCondition.isHeat[0] = true;
            enemyUnitRemain--;
            AbilityUseCheck(enemyUnitID[enemyUnitCount]);
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 3));
        }
        // enemy Unit�� ������ ���ϰ� Ÿ�� �޾��� ��
        else
        {
            playerAttackCount++;
            enemyCondition.isHeat[0] = true;
            AbilityUseCheck(enemyUnitID[enemyUnitCount]);
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 1)); // 1 enemy
        }
        currentTurn++;
        Debug.Log("���� ���� : " + currentTurn);
        //--------------------���� ����---------------------//
    }

    void ControllConditionGame(int checkNum) // 0 player , 1 enemy
    {
        //------------����� Ȯ�� �� ���� ����-------------//
        // ���� ���� �о�� position ����
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

        //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
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

    // ���� �� Paneló��
    void EndFightCheck()
    {
        if(playerUnitRemain == 0) // player���� �й� �� ���� ���ÿ� �������� ��� �й�� ó��
        {
            enemyWinPanel.SetActive(true);
            BGM_Sound.Stop();
            SFX_Sound.clip = playerDefeatSound;
            SFX_Sound.Play();
        }
        else if(enemyUnitRemain == 0) // enemy���� �й�
        {
            playerWinPanel.SetActive(true);
            BGM_Sound.Stop();
            SFX_Sound.clip = playerWinSound;
            SFX_Sound.Play();
        }
    }

    // Attack �ǽ��Ҷ����� �Լ� ����
    void AbilityUseCheck(float ID)
    {   
        // attack animal field where detective
        // player
        if(ID > 0 && ID <= 100) // ���� ������ unit�� �ɷ� ���� Ȯ�� ����
        {
            if(ID > 10 && ID <= 20 && unitCondition[ID].canUseAility == true)
            {
                unitCondition[ID].isAbility[0] = true;
                Debug.Log("Ability Set true : " + ID);
            }
            // �ܺ� unit�� �ɷ� ��� �ʿ� ���� ID ��
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

    // �ɷ��� True�� ���� �� �����ؾ���. case�� ����
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
                // ������Ʈ�� ó���� �ִ���
                if(count == 0)
                {
                    temp = unitCondition[playerUnitID[count + 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    playerUiBox.transform.GetChild(count + 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                    playerUiBox.transform.GetChild(count + 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + temp.Attack;
                }
                // ������Ʈ�� �������� �ִ���
                else if(count == playerEndCount)
                {
                    temp = unitCondition[playerUnitID[count - 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    playerUiBox.transform.GetChild(count - 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + temp.Heart;
                    playerUiBox.transform.GetChild(count - 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + temp.Attack;
                }
                // �̻�! �翷�� �ɷ� �ο�
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
                // ������Ʈ�� ó���� �ִ���
                if (count == 0)
                {
                    temp = unitCondition[enemyUnitID[count + 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    enemyUiBox.transform.GetChild(count + 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                    enemyUiBox.transform.GetChild(count + 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Attack;
                }
                // ������Ʈ�� �������� �ִ���
                else if (count == enemyEndCount)
                {
                    temp = unitCondition[enemyUnitID[count - 1]].animals[0].GetComponent<animalID>();
                    temp.Attack += 1;
                    temp.Heart += 1;
                    enemyUiBox.transform.GetChild(count - 1).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Heart;
                    enemyUiBox.transform.GetChild(count - 1).transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + currentAnimal.GetComponent<animalID>().Attack;
                }
                // �̻�! �翷�� �ɷ� �ο�
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

    // �߰��� �迭�� �׾��� �� array ������ ������ ���� ���� �ں��� ä���.
    // �Ű������� ��ɷ� ä�����ϳ�.. ���� ī��Ʈ ��ȣ�� �ѱ���!
    // ���� ���� ī��Ʈ�Դϴ�.
    void UnitArraySet(int unitCount, int checkNum) // checkNum 0 == player , 1 == enemy
    {
        Condition currentObj;

        if (checkNum == 0)
        {
            currentObj = unitCondition[playerUnitID[unitCount]];
            if (unitCount < playerUnitCount + playerUnitRemain - ++playerLastArray && unitCount > playerUnitCount) // playerUnitArrays ���̿� ���� ���
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                // ����� ������ �ڿ������ ������ ä���ִ� ����.
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

                //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
                if (unitCondition[playerUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --playerUnitRemain;
                }
            }
            else if (unitCount == playerUnitCount) // �տ� �ִ� ������ ������� ���
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                if (unitCondition[playerUnitID[playerUnitCount]].isDead[0])
                {
                    playerBox.transform.DOMoveX(playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
                    playerUiBox.GetComponent<RectTransform>().DOAnchorPosX(75 * playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
                }
                //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
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
            else // �ǵڿ� �ִ� ������ �׾������
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));
                //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
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
            if (unitCount < enemyUnitCount + enemyUnitRemain - (++enemyLastArray) && unitCount > enemyUnitCount) // playerUnitArrays ���̿� ���� ���
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                // ����� ������ �ڿ������ ������ ä���ִ� ����.
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

                //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
                if (unitCondition[enemyUnitID[unitCount]].isDead[0])
                {
                    currentObj.isHeat.RemoveAt(0);
                    currentObj.isDead.RemoveAt(0);
                    currentObj.animals.RemoveAt(0);
                    --enemyUnitRemain;
                }
            }
            else if (unitCount == enemyUnitCount) // �տ� �ִ� ������ ������� ���
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));

                if (unitCondition[enemyUnitID[enemyUnitCount]].isDead[0])
                {
                    enemyBox.transform.DOMoveX(enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
                    enemyUiBox.GetComponent<RectTransform>().DOAnchorPosX(-75 * enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
                }
                //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
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
            else // �ǵڿ� �ִ� ������ �׾������
            {
                StartCoroutine(StartDieAnim(currentObj.UnitReturn(), checkNum, unitCount));
                //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
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
        yield return new WaitForSeconds(0.01f); // ������
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
        yield return new WaitForSeconds(0.01f); // ������

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