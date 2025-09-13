using System.Collections;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public CSVReader csvReader;
    public SortManager sortManager;

    #region UI
    [Header("패널")]
    public GameObject mainPanel;
    public GameObject missionPanel;
    public GameObject characterPanel;
    public GameObject clanPanel;
    public GameObject storePanel;
    public GameObject gachaPanel;
    public GameObject loadingPanel;
    public GameObject menuBar;

    [Header("메인 화면")]
    public Image lobbyCharacter;
    public Text userLevelTxt;
    public Text userNameTxt;
    public Slider expBar;

    public GameObject mainItemBar;
    public Text mainEnergyTxt;
    public Text mainCoinTxt;
    public Text mainJewelTxt;

    public GameObject eventScrollView_R;
    public Image pickUpImage;
    public Text pickUpName;

    public GameObject lobbyPanel;
    public GameObject mailPanel;
    public GameObject optionPanel;
    public string standingNum;

    [Header("캐릭터 리스트")]
    public GameObject characterSelectPanel;
    public GameObject informationWin;
    public GameObject levelUpWin;
    public GameObject skillSetWin;

    [Header("캐릭터 화면")]
    public Image characterStanding;
    public Text characterName;
    public Text characterLevel;
    public GameObject characterStar;
    public Sprite starPrefabA;
    public Sprite starPrefabB;
    public Text curExpTxt;
    public Slider curExpBar;

    [Header("캐릭터 화면 - 스탯")]
    public Text characterHP;
    public Text characterRange;
    public Text characterAtk;
    public Text characterMAtk;
    public Text characterDef;
    public Text characterMDef;
    public Text[] potionCntTxt = new Text[4];

    [Header("캐릭터 화면 - 스킬")]
    public GameObject[] patterns = new GameObject[6];
    public GameObject[] skills = new GameObject[4];

    [Header("캐릭터 화면 - 별")]
    public Image pieceImage;
    public Text pieceName;
    public Text pieceCount;
    public Slider pieceSlider;

    [Header("퀘스트 화면")]
    public GameObject questPanel;
    public GameObject areaSelectPanel;
    public GameObject areaPanel;
    public GameObject stagePanel;
    public GameObject window1;
    public GameObject window2;
    public GameObject cautionPanel;
    public GameObject storyPanel;
    public GameObject dungeonPanel;
    public GameObject arenaPanel;

    [Header("상단 바")]
    public GameObject itemBar;
    public Text titleText;
    public Text energyTxt;
    public Text coinTxt;
    public Text jewelTxt;

    [Header("특수 상점")]
    public GameObject energyStore;
    public GameObject coinStore;
    public GameObject jewelStore;
    public GameObject popUp;
    public Text popUpText;

    [Header("옵션")]
    public InputField codeInput;

    #endregion

    private void Awake()
    {
        instance = this;
        csvReader = CSVReader.instance;
        if (PlayerPrefs.HasKey("Standing") == false)
        {
            standingNum = "1";
            PlayerPrefs.SetString("Standing", "1");
        }
        else
        {
            standingNum = PlayerPrefs.GetString("Standing");
        }
    }

    private void Start()
    {
        StartCoroutine(SetPlayerDataToUI());
        SetDailyPickUp();
    }

    public IEnumerator SetPlayerDataToUI()
    {
        // csvReader가 준비될 때까지 대기
        while (csvReader == null)
            yield return null;

        ChangeStanding();

        userNameTxt.text = csvReader.playerData.name;
        userLevelTxt.text = csvReader.playerData.lv.ToString();

        mainEnergyTxt.text = PlayerPrefs.GetInt("Energy") + "/200";
        mainCoinTxt.text = csvReader.playerData.coin.ToString();
        mainJewelTxt.text = csvReader.playerData.jewel.ToString();

        energyTxt.text = PlayerPrefs.GetInt("Energy") + "/200";
        coinTxt.text = csvReader.playerData.coin.ToString();
        jewelTxt.text = csvReader.playerData.jewel.ToString();
    }

    //코루틴 필요한 버튼들은 이 함수로 사용
    public void OnBtn() 
    {
        PlayerPrefs.Save();
        string btnName = EventSystem.current.currentSelectedGameObject.name;

        switch (btnName)
        {
            case "BackBtn":
                if (characterSelectPanel.gameObject.activeSelf)
                    characterSelectPanel.SetActive(false);
                else if (areaSelectPanel.gameObject.activeSelf)
                    areaSelectPanel.SetActive(false);
                else if (areaPanel.gameObject.activeSelf)
                {
                    areaPanel.GetComponent<AreaPanel>().ResetStageBtn();
                    areaPanel.SetActive(false);
                }
                else if (
                    storyPanel.gameObject.activeSelf
                    || dungeonPanel.gameObject.activeSelf
                    || arenaPanel.gameObject.activeSelf
                    )
                {
                    storyPanel.SetActive(false);
                    dungeonPanel.SetActive(false);
                    arenaPanel.SetActive(false);

                    titleText.text = "퀘스트";
                    ScriptManager.instance.QuestNPC();
                }
                else StartCoroutine(OnMainPanelBtn());
                break;
            case "MissionPanelBtn":
                StartCoroutine(OnMissionPanelBtn());
                break;
            case "CharacterPanelBtn":
                StartCoroutine(OnCharacterPanelBtn());
                break;
            case "ClanPanelBtn":
                StartCoroutine(OnClanPanelBtn());
                break;
            case "StorePanelBtn":
                StartCoroutine(OnStorePanelBtn());
                break;
            case "GachaPanelBtn":
            case "Gacha1":
                StartCoroutine(OnGachaPanelBtn());
                break;
            case "QuestPanelBtn":
                StartCoroutine(OnQuestPanelBtn());
                break;
            case "CharacterBtn(Clone)":
                OnCharacterSelectBtn();
                break;
            case "StoryBtn":
                OnStoryPanelBtn();
                break;
            case "DungeonBtn":
                OnDungeonPanelBtn();
                break;
            case "ArenaBtn":
                OnArenaPanelBtn();
                break;
            default:
                break;
        }
    }

    #region ItemBtn
    public void OnOpenMailBtn()
    {
        mailPanel.SetActive(true);
    }
    public void OnCloseMailBtn()
    {
        mailPanel.SetActive(false);
    }
    public void OnOpenOptionBtn()
    {
        optionPanel.SetActive(true);
    }
    public void OnCloseOptionBtn()
    {
        optionPanel.SetActive(false);
    }
    #endregion

    #region MainPanel
    public void OnLobbyPanel()
    {
        sortManager.isLobby = true;
        lobbyPanel.SetActive(true);
    }

    public void SetDailyPickUp()
    {
        int pickUp = PlayerPrefs.GetInt("DailyPickUp");
        pickUpImage.sprite = Resources.Load<Sprite>("Standing/" + pickUp);
        pickUpName.text = csvReader.characters[pickUp - 1].name;
    }

    public void ChangeStanding()
    {
        lobbyCharacter.sprite = Resources.Load<Sprite>("Standing/" + standingNum);
        ScriptManager.instance.SetLobbyMessage();
        lobbyPanel.SetActive(false);
    }

    public void SetLobbyBG()
    {
        //mainPanel.GetComponent<Image>().sprite = 
        sortManager.isLobby = false;
        lobbyPanel.SetActive(false);
    }
    public void CloseLobbyPanel()
    {
        sortManager.isLobby = false;
        lobbyPanel.SetActive(false);
    }

    #endregion

    #region CharacterPanelBtn
    public void OnCharacterSelectBtn()
    {
        characterSelectPanel.SetActive(true);
    }

    public void OnInformationBtn()
    {
        levelUpWin.SetActive(false);
        skillSetWin.SetActive(false);
        informationWin.SetActive(true);
    }
    public void OnLevelUpBtn()
    {
        skillSetWin.SetActive(false);
        informationWin.SetActive(false);
        levelUpWin.SetActive(true);
    }
    public void OnSkillSetBtn()
    {
        informationWin.SetActive(false);
        levelUpWin.SetActive(false);
        skillSetWin.SetActive(true);
    }
    #endregion

    #region QuestPanelBtn
    public void OnQuestBtn()
    {
        areaSelectPanel.SetActive(true);
    }
    public void OnAreaSelectBtn()
    {
        areaSelectPanel.SetActive(false);

        areaPanel.SetActive(true);
        areaPanel.GetComponent<AreaPanel>().SetAreaData();
        areaPanel.GetComponent<AreaPanel>().SetAreaUI();
        areaPanel.GetComponent<AreaPanel>().ResetStageBtn();
        areaPanel.GetComponent<AreaPanel>().SetStageBtn();

    }
    public void OnStagePanelBtn()
    {
        stagePanel.SetActive(true);
        StartCoroutine(areaPanel.GetComponent<AreaPanel>().SetStageWindow1());
    }
    public void OnStagePanelCancelBtn()
    {
        stagePanel.SetActive(false);
    }
    public void OnStagePanelNextBtn()
    {
        window2.SetActive(true);
        StartCoroutine(areaPanel.GetComponent<AreaPanel>().SetStageWindow2());
    }
    public void OnStagePanelCancelBtn2()
    {
        window2.SetActive(false);
    }
    public void OnPlayBattleBtn()
    {
        int cnt = 0;
        for (int i = 0; i < 5; i++)
        {
            if (csvReader.playerData.lineUp[i] != 0) cnt++;
        }

        if (cnt > 0)
        {
            int energy = PlayerPrefs.GetInt("Energy");
            PlayerPrefs.SetInt("Energy", energy - 5);

            StartCoroutine(SetPlayerDataToUI());
            StartCoroutine(LoadScene());
        }
        else
        {
            cautionPanel.SetActive(true);
            Debug.Log("라인업에 등록된 캐릭터가 없음!");
        }
    }
    public void OffCautionPanel()
    {
        cautionPanel.SetActive(false);
    }
    IEnumerator LoadScene()
    {
        // loadingPanel 활성화 후 페이드 인 (Alpha 0 -> 1)
        loadingPanel.SetActive(true);
        Image panelImage = loadingPanel.GetComponent<Image>();
        Color panelColor = panelImage.color;
        panelColor.a = 0f;
        panelImage.color = panelColor;

        while (panelColor.a < 1f)
        {
            panelColor.a += Time.deltaTime / 0.5f;  // 약 0.5초 동안 실행
            panelImage.color = panelColor;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // 두번째 이미지 페이드 인 처리 (Alpha 0 -> 1)
        // 주의: "FindChild"는 이제 "Find"를 사용하는 것이 좋습니다.
        Image disappearImage = loadingPanel.transform.Find("Image").GetComponent<Image>();
        Color disappearColor = disappearImage.color;
        disappearColor.a = 0f;
        disappearImage.color = disappearColor;

        while (disappearColor.a < 1f)
        {
            disappearColor.a += Time.deltaTime / 0.5f;  // 약 0.5초 동안 실행
            disappearImage.color = disappearColor;
            yield return null;
        }

        SceneManager.LoadScene("BattleScene");
    }

    #endregion

    #region LoadingPanel
    IEnumerator AppearLoadingPanel()
    {
        loadingPanel.SetActive(true);
        UnityEngine.Color color = loadingPanel.GetComponent<Image>().color;
        color.a = 0;

        Image loadImage = loadingPanel.transform.GetChild(0).GetComponent<Image>();
        UnityEngine.Color cgColor = loadImage.color;
        cgColor.a = 0;

        Sprite randomCG = GameManager.instance.allCGs[Random.Range(0, GameManager.instance.allCGs.Length)];
        loadImage.sprite = randomCG;
        loadImage.preserveAspect = true;

        while (color.a < 1f)
        {
            color.a += Time.deltaTime / 0.5f;   //1초동안 실행
            loadingPanel.GetComponent<Image>().color = color;

            cgColor.a += Time.deltaTime / 0.5f;
            loadImage.color = cgColor;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

    }
    IEnumerator DisappearLoadingPanel()
    {
        UnityEngine.Color color = loadingPanel.GetComponent<Image>().color;

        Image loadImage = loadingPanel.transform.GetChild(0).GetComponent<Image>();
        UnityEngine.Color cgColor = loadImage.color;

        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / 0.5f;   //1초동안 실행
            loadingPanel.GetComponent<Image>().color = color;

            cgColor.a -= Time.deltaTime / 0.5f;
            loadImage.color = cgColor;
            yield return null;
        }

        loadingPanel.SetActive(false);
    }
    IEnumerator OnMainPanelBtn()
    {
        yield return StartCoroutine(AppearLoadingPanel());

        itemBar.SetActive(false);
        missionPanel.SetActive(false);
        characterPanel.SetActive(false);
        clanPanel.SetActive(false);
        storePanel.SetActive(false);
        gachaPanel.SetActive(false);
        questPanel.SetActive(false);

        menuBar.SetActive(true);
        menuBar.transform.position = new Vector2(0, 0);

        ScriptManager.instance.SetLobbyMessage();

        yield return StartCoroutine(DisappearLoadingPanel());
    }
    IEnumerator OnMissionPanelBtn()
    {
        yield return StartCoroutine(AppearLoadingPanel());

        menuBar.SetActive(false);
        characterPanel.SetActive(false);
        clanPanel.SetActive(false);
        storePanel.SetActive(false);
        gachaPanel.SetActive(false);

        itemBar.SetActive(true);
        missionPanel.SetActive(true);
        titleText.text = "미션";

        ScriptManager.instance.MissionNPC();

        yield return StartCoroutine(DisappearLoadingPanel());
    }
    IEnumerator OnCharacterPanelBtn()
    {
        yield return StartCoroutine(AppearLoadingPanel());

        missionPanel.SetActive(false);
        menuBar.SetActive(false);
        clanPanel.SetActive(false);
        storePanel.SetActive(false);
        gachaPanel.SetActive(false);

        itemBar.SetActive(true);
        characterPanel.SetActive(true);
        titleText.text = "캐릭터";

        yield return StartCoroutine(DisappearLoadingPanel());
    }
    IEnumerator OnClanPanelBtn()
    {
        yield return StartCoroutine(AppearLoadingPanel());

        missionPanel.SetActive(false);
        menuBar.SetActive(false);
        characterPanel.SetActive(false);
        storePanel.SetActive(false);
        gachaPanel.SetActive(false);

        itemBar.SetActive(true);
        clanPanel.SetActive(true);
        titleText.text = "길드";

        ScriptManager.instance.ClanNPC();

        yield return StartCoroutine(DisappearLoadingPanel());
    }
    IEnumerator OnStorePanelBtn()
    {
        yield return StartCoroutine(AppearLoadingPanel());

        missionPanel.SetActive(false);
        menuBar.SetActive(false);
        characterPanel.SetActive(false);
        clanPanel.SetActive(false);
        gachaPanel.SetActive(false);

        itemBar.SetActive(true);
        storePanel.SetActive(true);
        titleText.text = "상점";

        ScriptManager.instance.StoreNPC();

        yield return StartCoroutine(DisappearLoadingPanel());
    }
    IEnumerator OnGachaPanelBtn()
    {
        yield return StartCoroutine(AppearLoadingPanel());

        missionPanel.SetActive(false);
        menuBar.SetActive(false);
        characterPanel.SetActive(false);
        clanPanel.SetActive(false);
        storePanel.SetActive(false);

        itemBar.SetActive(true);
        gachaPanel.SetActive(true);
        titleText.text = "모집";

        yield return StartCoroutine(DisappearLoadingPanel());
    }
    IEnumerator OnQuestPanelBtn()
    {
        yield return StartCoroutine(AppearLoadingPanel());

        missionPanel.SetActive(false);
        menuBar.SetActive(false);

        itemBar.SetActive(true);
        questPanel.SetActive(true);
        titleText.text = "퀘스트";

        ScriptManager.instance.QuestNPC();

        yield return StartCoroutine(DisappearLoadingPanel());
    }
    public void OnStoryPanelBtn()
    {
        titleText.text = "스토리";
        storyPanel.SetActive(true);

        ScriptManager.instance.StoryNPC();
    }
    public void OnDungeonPanelBtn()
    {
        titleText.text = "던전";
        dungeonPanel.SetActive(true);

        ScriptManager.instance.DungeonNPC();
    }
    public void OnArenaPanelBtn()
    {
        titleText.text = "아레나";
        arenaPanel.SetActive(true);

        ScriptManager.instance.ArenaNPC();
    }
    #endregion

    #region CharacterSelectPanel

    public void SetCharacterSelectPanel(CharacterData data)
    {
        characterStanding.sprite = Resources.Load<Sprite>("Standing/" + data.standingSprite);
        characterName.text = data.name;
        characterLevel.text = $"LV.{data.lv}";
        //characterStar;
        for (int i = 0; i < 5; i++)
        {
            Image starImage = characterStar.transform.GetChild(i).GetComponent<Image>();
            if (i < data.star) starImage.sprite = starPrefabA;
            else starImage.sprite = starPrefabB;
        }

        // 새 경험치 계산
        int neededExp = GrowthManager.instance.GetExpForLevel(data.lv);
        int curExp = (int)data.exp;

        curExpTxt.text = $"현재 {curExp}EXP - 레벨업까지 {neededExp - curExp}EXP";
        curExpBar.value = (float)curExp / neededExp;

        // 스탯 갱신
        GrowthManager.instance.data = data;
        GrowthManager.instance.CalculateCurrentStats(data);

        characterHP.text = data.hp.ToString();
        characterRange.text = data.range.ToString();
        characterAtk.text = data.atk.ToString();
        characterMAtk.text = data.mAtk.ToString();
        characterDef.text = data.def.ToString();
        characterMDef.text = data.mDef.ToString();

        for (int i = 0; i < potionCntTxt.Length; i++)
        {
            potionCntTxt[i].text = csvReader.items[i + 61].cnt.ToString();
        }
    }

    public void SetCharacterSkillPage(CharacterData data)
    {
        //스킬 패턴 UI
        for (int i = 0; i < patterns.Length; i++)
        {
            SkillData curSkill;
            
            if (data.pattern[i] != 0) //스킬 패턴일 때
            {
                curSkill = csvReader.skills.Find(x => x.id == data.skill[data.pattern[i] - 1]);
                patterns[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + GetSkillType(curSkill));
                patterns[i].transform.GetComponentInChildren<Text>().text = data.pattern[i].ToString();
            }
            else //휴식 패턴일 때
            {
                curSkill = null;
                patterns[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icon_rest");
                patterns[i].transform.GetComponentInChildren<Text>().text = "휴식";
            }
        }

        //스킬 UI
        for (int i = 0; i < skills.Length; i++)
        {
            SkillData curSkill;

            if (i == 0)
            {
                curSkill = csvReader.skills.Find(x => x.id == data.exSkill);
            }
            else
            {
                curSkill = csvReader.skills.Find(x => x.id == data.skill[i - 1]);
            }

            skills[i].transform.GetChild(1).GetComponent<Image>().sprite
                = Resources.Load<Sprite>("Sprites/" + GetSkillType(curSkill));
            skills[i].transform.GetChild(2).GetComponent<Text>().text
                = curSkill.name;
            skills[i].transform.GetChild(3).GetComponent<Text>().text
                = curSkill.description.Replace("\\n", "\n");
            skills[i].transform.GetChild(4).GetComponent<Text>().text
                = $"Lv {curSkill.lv}";
            skills[i].transform.GetChild(5).GetComponent<Text>().text
                = $"G {curSkill.lv * 500}";
        }
    }

    string GetSkillType(SkillData skill)
    {
        string t;

        switch (skill.typeA)
        {
            case "atk":
                t = "Icon_atk";
                break;

            case "mAtk":
                t = "Icon_matk";
                break;

            case "buffAtk":
            case "buffMAtk":
            case "buffDef":
            case "buffMDef":
            case "buffCri":
            case "buffMiss":
                t = "Icon_buff";
                break;

            case "debuffAtk":
            case "debuffMAtk":
            case "debuffDef":
            case "debuffMDef":
            case "debuffCri":
            case "debuffMiss":
                t = "Icon_debuff";
                break;

            case "healHp":
                t = "Icon_hp";
                break;
            case "healMp":
                t = "Icon_mp";
                break;
            case "stun":
                t = "Icon_stun";
                break;
            case "fire":
                t = "Icon_burn";
                break;
            case "decoy":
            case "joke":
                t = "Icon_decoy";
                break;
            case "unDebuff":
                t = "Icon_clear";
                break;
            default:
                t = "Icon_rest";
                break;
        }

        return t;
    }

    public void SetCharacterStarPage(CharacterData data)
    {
        ItemData curPiece = csvReader.items[data.id - 1];
        int neededPiece = GrowthManager.instance.GetRequiredPiece(data.star);

        pieceImage.sprite = Resources.Load<Sprite>("Face/" + data.faceSprite);
        pieceName.text = $"{data.name}의 카드 조각";
        pieceCount.text = $"({curPiece.cnt}/{neededPiece})";
        pieceSlider.value = (float)curPiece.cnt / neededPiece;
    }

    #endregion

    #region SpecialStore
    public void ItemPopUp(string itemName)
    {
        StartCoroutine(SetPlayerDataToUI()); //UI 세팅

        popUp.SetActive(true);
        popUpText.text = itemName + " 구매 완료!";
        StartCoroutine(ClosePopUp());
    }

    IEnumerator ClosePopUp()
    {
        yield return new WaitForSeconds(1f);
        popUp.SetActive(false);
    }

    public void WarningPopUp()
    {
        popUp.SetActive(true);
        popUpText.text = "재화가 부족합니다!";
        StartCoroutine(ClosePopUp());
    }
    public void GachaPopUp(string characterName)
    {
        popUp.SetActive(true);
        popUpText.text = characterName + " 획득!";
        StartCoroutine(ClosePopUp());
    }
    public void EnergyBtn01()
    {
        if (CSVReader.instance.playerData.coin < 5000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -5000);
        int curEnergy = PlayerPrefs.GetInt("Energy");
        curEnergy += 50;
        PlayerPrefs.SetInt("Energy", curEnergy);

        string itemName = "체력 +50";
        ItemPopUp(itemName);
    }
    public void EnergyBtn02()
    {
        if (CSVReader.instance.playerData.coin < 10000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -10000);
        int curEnergy = PlayerPrefs.GetInt("Energy");
        curEnergy += 100;
        PlayerPrefs.SetInt("Energy", curEnergy);

        string itemName = "체력 +100";
        ItemPopUp(itemName);
    }
    public void EnergyBtn03()
    {
        if (CSVReader.instance.playerData.coin < 20000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -20000);
        int curEnergy = PlayerPrefs.GetInt("Energy");
        curEnergy += 200;
        PlayerPrefs.SetInt("Energy", curEnergy);

        string itemName = "체력 +200";
        ItemPopUp(itemName);
    }
    public void EnergyBtn04()
    {
        if (CSVReader.instance.playerData.coin < 50000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -50000);
        int curEnergy = PlayerPrefs.GetInt("Energy");
        curEnergy += 500;
        PlayerPrefs.SetInt("Energy", curEnergy);

        string itemName = "체력 +500";
        ItemPopUp(itemName);
    }
    public void CoinBtn01()
    {
        if (CSVReader.instance.playerData.jewel < 100)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Jewel", -100);
        CSVReader.instance.ChangePlayerStat("Coin", 5000);

        string itemName = "5000 코인";
        ItemPopUp(itemName);
    }
    public void CoinBtn02()
    {
        if (CSVReader.instance.playerData.jewel < 200)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Jewel", -200);
        CSVReader.instance.ChangePlayerStat("Coin", 10000);

        string itemName = "10000 코인";
        ItemPopUp(itemName);
    }
    public void CoinBtn03()
    {
        if (CSVReader.instance.playerData.jewel < 1000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Jewel", -1000);
        CSVReader.instance.ChangePlayerStat("Coin", 50000);

        string itemName = "50000 코인";
        ItemPopUp(itemName);
    }
    public void CoinBtn04()
    {
        if (CSVReader.instance.playerData.jewel < 2000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Jewel", -2000);
        CSVReader.instance.ChangePlayerStat("Coin", 100000);

        string itemName = "100000 코인";
        ItemPopUp(itemName);
    }

    public void Store_energy()
    {
        energyStore.SetActive(true);
    }
    public void Store_energy_Off()
    {
        energyStore.SetActive(false);
    }
    public void Store_coin()
    {
        coinStore.SetActive(true);
    }
    public void Store_coin_Off()
    {
        coinStore.SetActive(false);
    }
    public void Store_jewel()
    {
        jewelStore.SetActive(true);
    }
    public void Store_jewel_Off()
    {
        jewelStore.SetActive(false);
    }
    #endregion

    public void JewelCode()
    {
        string code = codeInput.text;
        switch (code)
        {
            case "030608":
                csvReader.ChangePlayerStat("Jewel", 1000);
                break;
            case "050429":
                csvReader.ChangePlayerStat("Jewel", 10000);
                break;
            default:
                break;
        }

        StartCoroutine(SetPlayerDataToUI());
    }

}
