using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaPanel : MonoBehaviour
{
    public CSVReader csv;
    public UIManager ui;
    public GameManager gm;
    public SkipManager sm;

    [SerializeField]
    public StageData selecetedStage;
    List<StageData> curAreaStages = new List<StageData>();
    List<GameObject> stageBtns = new List<GameObject>();

    [Header("Area Panel")]
    public int areaNum;
    public Text areaName;
    public Image areaImage;
    public Text areaStarTxt;
    public Slider areaStarSlider;

    public GameObject stageContent;
    public GameObject stageBtn;

    [Header("Stage Panel")]
    public Image stageImage;
    public Text stageName;
    public Image stageStar;
    public GameObject rewardSlots;
    public GameObject enemySlots;

    public GameObject window2;
    public List<CharacterButton> battleList = new List<CharacterButton>();
    public GameObject[] battleListBtn = new GameObject[5];

    [Header("Prefabs")]
    public Sprite basicSprite;
    public Sprite starOn;
    public Sprite starOff;

    void Awake()
    {
        if (csv == null)
            csv = FindObjectOfType<CSVReader>();
        if (gm == null)
            gm = FindObjectOfType<GameManager>();
    }


    public void SetAreaData()
    {
        curAreaStages.Clear();

        for (int i = 0; i < csv.stages.Count; i += 3)
        {
            if (csv.stages[i].stageNum / 10000 == areaNum)
            {
                curAreaStages.Add(csv.stages[i]);
            }
        }
    }

    public void SetAreaUI()
    {
        switch (areaNum)
        {
            case 1:
                areaName.text = "제 1지역 : 스페로 헌터 학교";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch1/BG_SchoolGround");
                break;
            case 2:
                areaName.text = "제 2지역 : 기계인형의 게이트";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch1/BG_Park_Erode");
                break;
            case 3:
                areaName.text = "제 3지역 : N-11 설산";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch2/BG_WinterRoad");
                break;
            case 4:
                areaName.text = "제 4지역 : 망령들의 연구소";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch2/BG_RuinUndergroundPassage_Night");
                break;
            case 5:
                areaName.text = "제 5지역 : 메마른 왕국";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch3/BG_TrinityClubRoom_Night");
                break;
            case 6:
                areaName.text = "제 6지역 : 별과 피라미드";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch3/BG_Wilderness_Night");
                break;
            case 7:
                areaName.text = "제 7지역 : 푸른 늑대의 교단";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch4/BG_BuildingFrontGate");
                break;
            case 8:
                areaName.text = "제 8지역 : 영원한 게이트";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch4/BG_RuinCenter");
                break;
            case 9:
                areaName.text = "제 9지역 : 재앙이 내린 도시";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch5/BG_DemolitionCity_Night");
                break;
            case 10:
                areaName.text = "제 10지역 : 최종 결전";
                areaImage.sprite = Resources.Load<Sprite>("Background/Ch5/BG_CS_PV4_106_2");
                break;
            default:
                break;
        }
    }

    public void SetStageBtn()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject btn = Instantiate(stageBtn, stageContent.transform);

            Button goBtn = btn.transform.Find("GoBtn").GetComponent<Button>();
            goBtn.onClick.AddListener(ui.OnStagePanelBtn);
            goBtn.onClick.AddListener(btn.GetComponent<StageButton>().GetSelectedStage);

            Text stageNum = btn.transform.Find("StageNum").GetComponent<Text>();
            Text stageName = btn.transform.Find("StageName").GetComponent<Text>();

            stageNum.text = areaNum.ToString() + " - " + (i + 1);
            stageName.text = curAreaStages[i].stageName;

            Transform starParent = btn.transform.Find("StageStar");
            for (int j = 0; j < starParent.childCount; j++)
            {
                Image img = starParent.GetChild(j).GetComponent<Image>();
                // j < 획득한 별 개수면 on, 아니면 off
                img.sprite = (j < curAreaStages[i].star) ? starOn : starOff;
            }

            btn.GetComponent<StageButton>().stageData = curAreaStages[i];
            btn.GetComponent<StageButton>().areaPanel = this;
            stageBtns.Add(btn);

            //현재 스테이지 스타 값과 이전 스테이지 스타 값이 0일때
            if(i != 0 && curAreaStages[i].star == 0 && curAreaStages[i-1].star == 0)
            {
                goBtn.interactable = false;
                stageNum.color = Color.gray;
                stageName.color = Color.gray;
                btn.GetComponent<Image>().color = Color.gray;
            }

        }
    }

    // 배틀 리스트 버튼 UI 갱신 및 이벤트 등록 메서드
    public void SetBattleListButton()
    {
        // battleListBtn의 모든 슬롯을 순회
        for (int i = 0; i < battleListBtn.Length; i++)
        {
            GameObject slotButton = battleListBtn[i];
            Image slotImage = slotButton.GetComponent<Image>();
            Button btn = slotButton.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();  // 중복 이벤트 제거

            if (i < battleList.Count)
            {
                // 등록된 전투 리스트에 캐릭터가 있으면 해당 캐릭터의 faceImage 스프라이트 적용
                slotImage.sprite = battleList[i].GetComponent<Image>().sprite;

                // 정확한 슬롯 캡쳐 (지역 변수)
                GameObject capturedButton = slotButton;
                btn.onClick.AddListener(() => RemoveBattleListButton(capturedButton));
            }
            else
            {
                // 등록되지 않은 슬롯은 기본 스프라이트 적용
                slotImage.sprite = basicSprite;
            }
        }

        battleList.Sort((a, b) => a.data.range.CompareTo(b.data.range));

        //플레이어 데이터에도 라인업 저장
        for (int i = 0; i < 5; i++)
        {
            string statName = "LineUp" + (i + 1);

            if (i < battleList.Count)
            {
                csv.ChangePlayerStat(statName, battleList[i].data.id);
            }
            else
            {
                csv.ChangePlayerStat(statName, 0);
            }
        }
    }

    // 클릭한 배틀 리스트 버튼을 통해 캐릭터 제거 및 버튼 이미지 초기화
    public void RemoveBattleListButton(GameObject clickedBtn)
    {
        // 전달받은 버튼의 Image 컴포넌트 사용
        Image clickedImg = clickedBtn.GetComponent<Image>();

        // battleList에서 clickedImg와 같은 스프라이트를 가진 캐릭터를 검색하여 제거
        // (동일한 캐릭터가 중복으로 있을 경우 한 번만 제거)
        for (int i = battleList.Count - 1; i >= 0; i--)
        {
            Image listImg = battleList[i].GetComponent<Image>();
            if (clickedImg.sprite == listImg.sprite)
            {
                battleList.RemoveAt(i);
                break;
            }
        }

        // 클릭한 슬롯의 이미지를 기본 스프라이트로 초기화
        clickedImg.sprite = basicSprite;

        // 전투 리스트 슬롯 UI 다시 갱신
        SetBattleListButton();
    }


    public void ResetStageBtn()
    {
        for(int i = 0; i < stageBtns.Count; i++)
        {
            Destroy(stageBtns[i]);
        }
        stageBtns.Clear();
    }

    public IEnumerator SetStageWindow1()
    {
        yield return new WaitForEndOfFrame();
        stageName.text = areaNum.ToString() + " - " + (selecetedStage.stageNum / 100 % 100) + " " + selecetedStage.stageName;

        for (int j = 0; j < stageStar.transform.childCount; j++)
        {
            var img = stageStar.transform.GetChild(j).GetComponent<Image>();
            img.sprite = (j < selecetedStage.star) ? starOn : starOff;
        }

        int index = csv.stages.FindIndex(x => x.stageNum == selecetedStage.stageNum);
        StageData newStage = csv.stages[index + 2];

        for (int i = 0; i < 3; i++)
        {
            gm.curStage[i] = csv.stages[index + i];
            Debug.Log(csv.stages[index + i].stageNum);
            Debug.Log(gm.curStage[i].stageNum);
        }

        for (int i = 0; i < 5; i++)
        {
            Text enemyName = enemySlots.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>();
            Image enemyImage = enemySlots.transform.GetChild(i).GetComponent<Image>();

            int index2 = csv.enemys.FindIndex(x => x.id.Equals(newStage.enemyId[i]));

            if (index2 == -1)
            {
                yield return null;
            }
            else
            {
                enemyName.text = csv.enemys[index2].name;
                enemyImage.sprite = Resources.Load<Sprite>($"Enemy/{csv.enemys[index2].id}");
            }
        }

        sm.stage = selecetedStage;
    }
    public IEnumerator SetStageWindow2()
    {
        yield return new WaitForEndOfFrame();

        battleList.Clear();
        List<Transform> items = window2.GetComponent<SortManager>().items;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < items.Count; j++)
            {
                if (items[j].gameObject.GetComponent<CharacterButton>().data.id == csv.playerData.lineUp[i])
                {
                    battleList.Add(items[j].gameObject.GetComponent<CharacterButton>());
                }
            }
        }

        SetBattleListButton();
    }
}
