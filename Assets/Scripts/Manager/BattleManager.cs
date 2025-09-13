using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    #region Defination
    public static BattleManager instance { get; private set; }

    public enum BattleState
    {
        ready = 0,
        playing,
        pause,
        win,
        clear,
        defeat
    }

    public BattleState state;

    [Header("UI")]
    public GameManager gm;
    public CSVReader csv;
    public SkillManager skillManager;
    public GameObject startPanel;
    public GameObject pausePanel;
    public GameObject clearPanel;
    public GameObject nextStagePanel;
    public GameObject getItemList;
    public SpriteRenderer BG;
    public Image standing;
    public TextMeshProUGUI script;
    public Text endingText;
    public Text speedTxt;
    public Text stageText;
    public Text timer;

    [Header("Prefab")]
    public GameObject getItem;
    public GameObject player;
    public GameObject enemy;
    public StageData stage;
    public Sprite jewelSprite;
    public Sprite coinSprite;
    public Sprite ticketSprite;
    public Sprite couponSprite;
    public Sprite poS;
    public Sprite poM;
    public Sprite poL;
    public Sprite poXL;
    public Sprite starOn;
    public Sprite starOff;

    [Header("List")]
    public List<CharacterData> lineUp = new List<CharacterData>();
    public List<GameObject> playerList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();

    [Header("Array")]
    public GameObject[] token = new GameObject[5];
    public Transform[] spawnPos = new Transform[5];
    public Transform[] enemySpawnPos = new Transform[5];
    public Image[] getStars = new Image[3];

    [Header("Guitar")]
    public List<bool> isArrived;
    public bool isPlaying = false;
    public bool isEnd = false;
    public int curRound = 0;
    private bool isCoroutineRunning = false;

    float curTime = 120f;
    int minute;
    int second;
    public bool timeTwice;
    bool isHard;

    Animator panelAnim;
    #endregion

    void Start()
    {
        instance = this;
        ResetData();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        csv = GameObject.Find("DataManager").GetComponent<CSVReader>();


        for (int i = 0; i < gm.curStage.Length; i++)
        {
            Debug.Log($"[GameManager] curStage[{i}] = {(gm.curStage[i] != null ? gm.curStage[i].stageNum.ToString() : "NULL")}");
        }

        state = BattleState.ready;
        panelAnim = startPanel.GetComponent<Animator>();

        SetStage(curRound);
        CreateEnemys();

        SetLineUp();
        CreatePlayers();

        UpdateStageText();
    }

    void Update()
    {
        switch (state)
        {
            case BattleState.ready:
                CheckIsReady();
                break;
            case BattleState.playing:
                StartCoroutine(UpdateTime());
                break;
            case BattleState.pause:
                break;
            case BattleState.win:
                if (!isCoroutineRunning)
                {
                    isCoroutineRunning = true;
                    curRound++;

                    if (curRound > 2)
                    {
                        state = BattleState.clear;
                        isCoroutineRunning = false;
                    }
                    else
                    {
                        StartCoroutine(GoNextStage());
                    }
                }
                break;
            case BattleState.clear:
                if (!isCoroutineRunning)
                {
                    isCoroutineRunning = true;

                    StartCoroutine(ClearStage());
                }
                break;
            case BattleState.defeat:
                if (!isCoroutineRunning)
                {
                    isCoroutineRunning = true;

                    StartCoroutine(GameOver());
                }
                break;
            default:
                break;
        }
    }

    #region GameSetting
    void SetLineUp()
    {
        lineUp.Clear();

        for (int i = 0; i < 5; i++)
        {
            if (csv.playerData.lineUp[i] > 0)
            {
                int index = csv.characters.FindIndex(x => x.id == csv.playerData.lineUp[i]);
                lineUp.Add(csv.characters[index]);
            }
        }
    }
    void CreatePlayers()
    {
        Debug.Log("Create Players");
        for (int i = 0; i < lineUp.Count; i++)
        {
            GameObject newPlayer = Instantiate(player, spawnPos[i]);
            Debug.Log($"라인업 {i}: {lineUp[i].name} → 토큰: {token[i]?.name}");

            PlayerController pc = newPlayer.GetComponent<PlayerController>();          
            pc.characterData = lineUp[i];

            for (int j = 0; j < 3; j++)
            {
                int id = pc.characterData.skill[j];
                SkillData newSkill = csv.skills.Find(x => x.id == id);
                pc.skillData.Add(newSkill);
            }

            pc.bm = this;
            pc.sm = skillManager;
            pc.token = token[i];
            token[i].SetActive(true);
            token[i].GetComponent<Image>().sprite 
                = Resources.Load<Sprite>("Face/" + pc.characterData.faceSprite);
            token[i].GetComponent<Button>().interactable = false;

            // Image 컴포넌트 가져오기
            SpriteRenderer img = newPlayer.GetComponent<SpriteRenderer>();
            Sprite loadedSprite = Resources.Load<Sprite>("SD/" + pc.characterData.standingSprite);

            // null 체크 후에만 스프라이트 적용
            if (loadedSprite != null)
            {
                img.sprite = loadedSprite;
            }

            playerList.Add(newPlayer);
        }

        // **추가**: X 좌표가 큰 순(전방 우선)으로 정렬
        playerList = playerList
            .OrderByDescending(go => go.transform.position.x)
            .ToList();
    }

    void SetStage(int i)
    {
        stage = gm.curStage[i];
        BG.sprite = Resources.Load<Sprite>("Background/" + stage.stageBG);
        stageText.text = "Stage " + (stage.stageNum / 10000) + "- " + (stage.stageNum / 100 - 100) + " (1/3)";
    }
    void CreateEnemys()
    {
        for (int i = 0; i < 5; i++)
        {
            if (stage.enemyId[i] != 0)
            {
                GameObject newEnemy = Instantiate(enemy, enemySpawnPos[i]);
                newEnemy.transform.position = new Vector2(stage.enemyPos[i], enemySpawnPos[i].position.y);

                PlayerController pc = newEnemy.GetComponent<PlayerController>();
                pc.characterData = csv.enemys[stage.enemyId[i] - 1];

                for (int j = 0; j < 3; j++)
                {
                    int id = pc.characterData.skill[j];
                    if (id != 0)
                    {
                        SkillData newSkill = csv.skills.Find(x => x.id == id);
                        pc.skillData.Add(newSkill);
                    }
                }

                pc.bm = this;
                pc.sm = skillManager;
                pc.isEnemy = true;

                //나중에 이거 데이터 받아와서 적 이미지 넣도록 바꿔야 함
                newEnemy.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Enemy/{pc.characterData.id}");

                enemyList.Add(newEnemy);
            }
        }
        // **추가**: X 좌표가 작은 순(후방 우선)으로 정렬
        enemyList = enemyList
            .OrderBy(go => go.transform.position.x)
            .ToList();
    }
    void CheckIsReady()
    {
        if (isArrived.Count >= playerList.Count)
        {
            isArrived.Clear();
            state = BattleState.playing;
            isPlaying = true;

            panelAnim.SetTrigger("GameStart");
        }        
    }
    #endregion

    #region GamePlaying
    IEnumerator UpdateTime()
    {
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            timer.text = minute.ToString("00") + ":" + second.ToString("00");
            yield return null;

            if (curTime <= 0)
            {
                Debug.Log("시간 종료");
                curTime = 0;
                isEnd = true;
                StartCoroutine(GameOver());
                yield break;
            }
        }
    }

    IEnumerator GoNextStage()
    {
        isPlaying = false;
        yield return new WaitForSeconds(1f);
        nextStagePanel.SetActive(true);
        ResetData();

        yield return new WaitForSeconds(1.2f);

        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerController pc = playerList[i].GetComponent<PlayerController>();
            pc.state = PlayerController.CharacterState.Idle;

            playerList[i].transform.position = spawnPos[i].position;
            pc.isArrived = false;
        }

        SetStage(curRound);
        CreateEnemys();
        UpdateStageText();

        yield return new WaitForSeconds(0.2f);

        nextStagePanel.SetActive(false);
        state = BattleState.ready;
        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerController pc = playerList[i].GetComponent<PlayerController>();
            pc.state = PlayerController.CharacterState.Move;

        }

        isCoroutineRunning = false;
    }
    #endregion

    #region GameEnd
    //게임 클리어
    IEnumerator ClearStage()
    {
        isPlaying = false;
        isEnd = true;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerController pc = playerList[i].GetComponent<PlayerController>();
            pc.state = PlayerController.CharacterState.Idle;

        }

        Debug.Log("::::::스테이지 클리어!!!::::::::::");

        //클리어 타이틀 나오고
        //캐릭터 스탠딩+보상받은 아이템 패널 나오기
        GetReward();
        GetStar();
        clearPanel.SetActive(true);
        endingText.text = "Stage Clear";

        yield return new WaitForSeconds(0.5f);
        SetStanding(true);
    }

    //게임 오버 시
    IEnumerator GameOver()
    {
        isPlaying = false;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerController pc = playerList[i].GetComponent<PlayerController>();
            pc.state = PlayerController.CharacterState.Idle;
        }

        Debug.Log("::::::::게임 오버::::::::");

        //게임오버 타이틀 나오고
        //캐릭터 스탠딩+보상받은 아이템 패널 나오기
        clearPanel.SetActive(true);
        endingText.text = "Game Over";

        yield return new WaitForSeconds(0.5f);
        SetStanding(false);
    }

    void GetStar()
    {
        int getStarCnt;

        if(playerList.Count == 5)
        {
            getStarCnt = 3;
        }
        else if(playerList.Count == 4)
        {
            getStarCnt = 2;
        }
        else if(playerList.Count == 0)
        {
            getStarCnt = 0;
        }
        else
        {
            getStarCnt = 1;
        }

        if (gm.curStage[0].star < getStarCnt)
            csv.ChangeStageStar(gm.curStage[0].stageNum, getStarCnt);

        for (int i = 0;i < getStarCnt; i++)
        {
            getStars[i].sprite = starOn;
        }
    }

    void GetReward()
    {
        ItemManager im = gm.gameObject.GetComponent<ItemManager>();
        int itemCnt = 4;
        int s, m, l, xl, coin;
        im.GeneratePotions(stage.stageNum / 10000, out s, out m, out l, out xl);
        coin = im.GetCoins(stage.stageNum / 10000);

        //데이터 상에 추가하기
        csv.ChangePlayerStat("Jewel", 50);
        csv.ChangePlayerStat("Coin", coin);
        csv.ChangeItemCount(62, s);
        csv.ChangeItemCount(63, m);
        csv.ChangeItemCount(64, l);
        csv.ChangeItemCount(65, xl);
        csv.ChangeItemCount(66, 1);

        //클리어패널에 획득 아이템 보여주기
        if (m > 0) itemCnt++; if (l > 0) itemCnt++; if (xl > 0) itemCnt++;

        //n-10 스테이지고 최초 클리어일 때 3성 확정 쿠폰
        if ((gm.curStage[0].stageNum / 100 % 100) == 10 && gm.curStage[0].star == 0)
        {
            csv.ChangeItemCount(68, 1);
            itemCnt++;
        }

        for (int i = 0; i < itemCnt; i++)
        {
            GameObject newItem = Instantiate(getItem, getItemList.transform);
            Image icon = newItem.transform.GetChild(0).GetComponent<Image>();
            Text name = newItem.transform.GetChild(1).GetComponent<Text>();

            switch (i)
            {
                case 0:
                    icon.sprite = jewelSprite;
                    name.text = "보석 x50";
                    break;
                case 1:
                    icon.sprite = coinSprite;
                    name.text = "골드 x" + coin.ToString();
                    break;
                case 2:
                    icon.sprite = ticketSprite;
                    name.text = "스킵 티켓 x1";
                    break;
                case 3:
                    icon.sprite = poS;
                    name.text = "경험치 포션(소) x" + s.ToString();
                    break;
                case 4:
                    icon.sprite = poS;
                    name.text = "경험치 포션(중) x" + m.ToString();
                    break;
                case 5:
                    icon.sprite = poS;
                    name.text = "경험치 포션(대) x" + l.ToString();
                    break;
                case 6:
                    icon.sprite = poS;
                    name.text = "경험치 포션(특대) x" + xl.ToString();
                    break;
                case 7:
                    icon.sprite = couponSprite;
                    name.text = "3성 확정 쿠폰 x1";
                    break;
                default:
                    break;
            }
        }

    }

    void ResetData()
    {
        clearPanel.SetActive(false);

        enemyList.Clear();
        isArrived.Clear();

        curTime = 120f;
    }

    void UpdateStageText()
    {
        stageText.text = "Stage " + (stage.stageNum / 10000) + "- " + ((stage.stageNum / 100) % 100)
            + " (" + (curRound + 1) + "/3)";
    }

    //게임 클리어 화면 스탠딩
    void SetStanding(bool isWin)
    {
        int rand = Random.Range(0, 5);
        string number = lineUp[rand].standingSprite;
        standing.sprite = Resources.Load<Sprite>("Standing/" + number);

        string mes;
        if (isWin) mes = csv.scripts[lineUp[rand].id - 1].win;
        else mes = csv.scripts[lineUp[rand].id - 1].defeat;

        StartCoroutine(CoTypewrite(mes));
    }
    private IEnumerator CoTypewrite(string message)
    {
        script.text = "";        // 텍스트 초기화
        foreach (char c in message)
        {
            script.text += c;    // 한 글자씩 추가
            yield return new WaitForSeconds(0.01f);
        }
    }

    #endregion

    #region Buttons
    //재도전 버튼
    public void OnReplayBtn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BattleScene");
    }

    //돌아가기 버튼
    public void OnGoHomeBtn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    //2배속 버튼
    public void OnTimeScaleBtn()
    {
        if (!timeTwice)
        {
            Time.timeScale = 2f;
            timeTwice = true;
            speedTxt.text = "X1";
        }
        else
        {
            Time.timeScale = 1f;
            timeTwice = false;
            speedTxt.text = "X2";
        }
    }

    //일시정지 버튼
    public void OnPauseBtn()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    //일시정지 해제
    public void OffPauseBtn()
    {
        if (!timeTwice) Time.timeScale = 1f;
        else Time.timeScale = 2f;

        pausePanel.SetActive(false);
    }
    #endregion
}
