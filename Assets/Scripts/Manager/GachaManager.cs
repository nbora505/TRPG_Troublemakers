using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    #region Defination

    CharacterData PUC;
    int time = 1;

    [Header("Panel")]
    public GameObject pickUpPanel;
    public GameObject commonPanel;
    public GameObject star3Panel;
    public GameObject gachaPanel;
    public GameObject selectPanel;

    [Header("PickUp UI")]
    public Image pickUpImage;
    public Text pickUpName;
    public Image pickUpStanding;
    public Text pickUpDescription;
    public Text gachaPoints;

    [Header("Select UI")]
    public GameObject selectWindow;
    public GameObject contents;
    public Text selectedCharacterTxt;
    public Button getSelectBtn;

    [Header("PopUp UI")]
    public GameObject popUp;
    public Text message;

    [Header("Animation")]
    public Animator anim;
    public GameObject animationPanel;
    public GameObject Scene00;
    public GameObject Scene01;
    public GameObject Scene02;
    public GameObject Scene03;
    public Button gachaBtn;
    public GameObject skipBtn;
    public GameObject finishGachaBtn;
    public GameObject newsParent;
    public GameObject[] stars = new GameObject[3];
    public Image Gwen;
    public Image bigNews;
    public Image standing;
    public Text characterName;
    public TextMeshProUGUI getScript;
    public Text getScript3;

    [Header("Prefab")]
    public GameObject newspaper;
    public Sprite[] newspapers = new Sprite[3];
    public GameObject getTxt;
    public StoreManager storeManager;
    public Sprite gwen1;
    public Sprite gwen2;
    public GameObject characterBtn;

    //데이터 상으로는 가챠 완료
    List<CharacterData> getList = new List<CharacterData>();
    List<int> boolList = new List<int>();
    CharacterData selectedCharacter;

    bool isCouponSelect;

    public enum GachaType
    {
        PickUp,
        Common,
        Star3
    }
    GachaType type;
    #endregion

    private void OnEnable()
    {
        SetDailyPickUp();
        OnPickUp();

        gachaPoints.text = PlayerPrefs.GetInt("gachaPoint").ToString();
        PlayerPrefs.Save();
    }

    public void SetDailyPickUp()
    {
        int pickUp = PlayerPrefs.GetInt("DailyPickUp");
        PUC = CSVReader.instance.characters[pickUp - 1];

        pickUpImage.sprite = Resources.Load<Sprite>("Standing/" + pickUp);
        pickUpName.text = PUC.name;

        pickUpStanding.sprite = Resources.Load<Sprite>("Standing/" + pickUp);
        pickUpDescription.text = $"★3 {PUC.name} 출현 확률 UP!";
    }

    IEnumerator PlayGacha()
    {
        // 1) 초기화: 이전 결과 및 UI 정리
        getList.Clear();
        boolList.Clear();
        for (int j = newsParent.transform.childCount - 1; j >= 0; j--)
        {
            Destroy(newsParent.transform.GetChild(j).gameObject);
        }

        //데이터 상으로는 가챠 완료
        PlayGachaInData(getList);


        //그웬 설정
        bool is3 = false;
        for (int i = 0; i < getList.Count; i++)
        {
            if (getList[i].star == 3) is3 = true;
        }
        if(is3)
        {
            int rand = Random.Range(0, 10);
            if(rand < 7) Gwen.sprite = gwen2;
            else Gwen.sprite = gwen1;
        }
        else
        {
            Gwen.sprite = gwen1;
        }

        //애니메이션 연출 ON
        animationPanel.SetActive(true);

        //버튼 누르면
        bool clicked = false;
        UnityAction handler = () => clicked = true;
        gachaBtn.onClick.AddListener(handler);
        yield return new WaitUntil(() => clicked);
        anim.SetTrigger("DoorClick");
        gachaBtn.onClick.RemoveListener(handler);
        clicked = false;

        skipBtn.SetActive(true);

        yield return new WaitForSeconds(3);
        Scene00.SetActive(false);
        Scene01.SetActive(false);

        //가챠 연출 1~10장
        for (int i = 0; i < time; i++)
        {
            GameObject newpaper = Instantiate(newspaper, newsParent.transform);
            newpaper.GetComponent<Image>().sprite = newspapers[getList[i].star - 1];
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1);
        Scene02.SetActive(true);

        //캐릭터 등장 연출 1~10회
        for (int i = 0; i < time; i++)
        {
            bigNews.sprite = newspapers[getList[i].star - 1];

            if (getList[i].star == 3)
            {
                anim.SetTrigger("Star3");
                getScript3.text = CSVReader.instance.scripts[getList[i].id - 1].get;
            }
            else 
            {
                if (i == 0) anim.SetTrigger("Character1");
                else anim.SetTrigger("Character2");
            }

            yield return new WaitForSeconds(0.5f);
            standing.sprite = Resources.Load<Sprite>("Standing/" + getList[i].standingSprite);
            characterName.text = getList[i].name;
            getScript.text = CSVReader.instance.scripts[getList[i].id - 1].get;

            for (int j = 0; j < stars.Length; j++)
                stars[j].SetActive(j < getList[i].star);

            // 다음 클릭 대기
            clicked = false;
            gachaBtn.onClick.AddListener(handler);
            yield return new WaitUntil(() => clicked);
            gachaBtn.onClick.RemoveListener(handler);
        }

        anim.SetTrigger("White");
        yield return new WaitForSeconds(0.5f);
        Scene03.SetActive(false);

        for (int i = 0; i < time; i++)
        {
            GameObject newpaper = newsParent.transform.GetChild(i).gameObject;
            newpaper.GetComponent<Image>().sprite = Resources.Load<Sprite>("Face/" + getList[i].faceSprite);
            GameObject txt = Instantiate(getTxt, newpaper.transform);

            GetCharacterPiece(i, txt);
        }

        finishGachaBtn.SetActive(true);

        yield return null;
    }

    void GetCharacterPiece(int i, GameObject txt)
    {
        if (boolList[i] == 3)
        {
            txt.GetComponent<Text>().text = "조각x50";
        }
        else if (boolList[i] == 2)
        {
            txt.GetComponent<Text>().text = "조각x10";
        }
        else if (boolList[i] == 1)
        {
            txt.GetComponent<Text>().text = "조각x1";
        }
        else
        {
            txt.GetComponent<Text>().text = "NEW!";
        }

    }

    void PlayGachaInData(List<CharacterData> getList)
    {
        //3성 - 3%(픽업 0.7% + 통상 2.3%)
        //2성 - 18.5%, 1성 - 78.5%
        List<CharacterData> data = CSVReader.instance.characters;

        for (int i = 0; i < time; i++)
        {
            CSVReader.instance.ChangePlayerStat("Jewel", -120);
            int rand = Random.Range(0, 1000);

            if (type == GachaType.Star3)
            {
                int getID = Random.Range(22, 59);
                getList.Add(data[getID]);

                CSVReader.instance.ChangeItemCount(68, -1);

                Debug.Log($"★3 {data[getID].name} 획득!");
            }
            else
            {
                if (rand < 30) //3성
                {
                    if (type == GachaType.PickUp && rand < 7) // 픽업
                    {
                        Debug.Log($"★3 {PUC.name} 획득!");
                        getList.Add(PUC);
                    }
                    else
                    {
                        int getID = Random.Range(22, 59);
                        getList.Add(data[getID]);
                        Debug.Log($"★3 {data[getID].name} 획득!");
                    }

                }
                else if (rand >= 30 && rand < 215) //2성
                {
                    int getID = Random.Range(10, 22);
                    getList.Add(data[getID]);
                    Debug.Log($"★2 {data[getID].name} 획득!");
                }
                else //1성
                {
                    if (i == 9) //10번째 가챠면
                    {
                        int getID = Random.Range(10, 22);
                        getList.Add(data[getID]);
                        Debug.Log($"★2 {data[getID].name} 획득!");
                    }
                    else
                    {
                        int getID = Random.Range(0, 10);
                        getList.Add(data[getID]);
                        Debug.Log($"★1 {data[getID].name} 획득!");
                    }
                }
            }

            //조각
            if (!getList[i].isGet)
            {
                boolList.Add(0);
                getList[i].isGet = true;
                CSVReader.instance.ChangeCharacterIsGet(getList[i].id, true);
            }
            else
            {
                boolList.Add(getList[i].star);

                if (getList[i].star == 3)
                {
                    CSVReader.instance.ChangeItemCount(61, 50);
                }
                else if (getList[i].star == 2)
                {
                    CSVReader.instance.ChangeItemCount(61, 10);
                }
                else if (getList[i].star == 1)
                {
                    CSVReader.instance.ChangeItemCount(61, 1);
                }
            }

            //가챠 포인트
            int gachaPoint = PlayerPrefs.GetInt("gachaPoint");
            PlayerPrefs.SetInt("gachaPoint", gachaPoint + 1);
            PlayerPrefs.Save();

        }
        gachaPoints.text = PlayerPrefs.GetInt("gachaPoint").ToString();
        //SortManager.instance.MakeCharacterBtns();
        //SortManager.instance.SortItems();
        StartCoroutine(UIManager.instance.SetPlayerDataToUI()); //UI 세팅
    }

    #region ★3
    public void UseCillingPointBtn()
    {
        isCouponSelect = false;

        int gachaPoint = PlayerPrefs.GetInt("gachaPoint");

        if( gachaPoint < 200 )
        {
            UIManager.instance.WarningPopUp();
            return;
        }

        OnSelectWindow();
    }

    public void UseSelectCouponBtn()
    {
        isCouponSelect = true;

        int coupon = CSVReader.instance.items[66].cnt;
        if (coupon < 1)
        {
            UIManager.instance.WarningPopUp();
            return;
        }
        OnSelectWindow();
    }

    public void OnSelectWindow()
    {
        selectedCharacter = null;
        selectedCharacterTxt.text = "선택 캐릭터 : - ";

        getSelectBtn.interactable = false;

        selectWindow.SetActive(true);
        MakeCharacterBtns();
    }

    void MakeCharacterBtns()
    {
        //초기화 먼저
        foreach (Transform child in contents.transform)
        {
            Destroy(child.gameObject);
        }

        List<CharacterData> data = CSVReader.instance.characters;
        for (int i = 22; i < data.Count; i++)
        {
            GameObject character = Instantiate(characterBtn, contents.transform);
            Button buttonComponent = character.GetComponent<Button>();
            CharacterData currentData = data[i];

            CharacterButton btn = character.GetComponent<CharacterButton>();
            if (btn != null)
                btn.SetUp(data[i]);

            for (int j = 0; j < 6; j++)
            {
                character.transform.GetChild(j).gameObject.SetActive(false);
            }

            // 람다를 이용해 버튼 클릭 시 함수 호출
            buttonComponent.onClick.AddListener(() =>
            {
                selectedCharacter = currentData;
                selectedCharacterTxt.text = $"선택 캐릭터 : {selectedCharacter.name}";
                getSelectBtn.interactable = true;
            });
        }
    }

    public void GetSelectedCharacter()
    {
        if (selectedCharacter == null) return;

        if (selectedCharacter.isGet)
        {
            CSVReader.instance.ChangeItemCount(61, 50);
        }
        else
        {
            selectedCharacter.isGet = true;
            CSVReader.instance.ChangeCharacterIsGet(selectedCharacter.id, true);
        }

        ReducePoint();

        selectWindow.SetActive(false);
        UIManager.instance.GachaPopUp(selectedCharacter.name);
        Debug.Log($"★3 {selectedCharacter.name} 획득!");
    }

    void ReducePoint()
    {
        if(isCouponSelect)
        {
            CSVReader.instance.ChangeItemCount(67, -1);
        }
        else
        {
            int gachaPoint = PlayerPrefs.GetInt("gachaPoint");

            PlayerPrefs.SetInt("gachaPoint", gachaPoint - 200);
            PlayerPrefs.Save();
            gachaPoints.text = PlayerPrefs.GetInt("gachaPoint").ToString();
        }
    }

    public void CloseSelectWindowBtn()
    {
        selectWindow.SetActive(false);
    }

    #endregion

    #region Buttons

    public void FinishGacha()
    {
        getList.Clear();

        Scene02.SetActive(false);
        Scene03.SetActive(false);
        Scene00.SetActive(true);
        Scene01.SetActive(true);
        animationPanel.SetActive(false);
        for (int i = newsParent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = newsParent.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        skipBtn.SetActive(false);
        finishGachaBtn.SetActive(false);
    }

    public void SkipGacha()
    {
        gachaBtn.onClick.RemoveAllListeners();
        StopAllCoroutines();
        StartCoroutine(Skipping());
    }
    IEnumerator Skipping()
    {
        anim.SetTrigger("White");
        yield return new WaitForSeconds(0.5f);
        Scene03.SetActive(false);
        Scene02.SetActive(true);

        for (int i = newsParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(newsParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < time; i++)
        {
            GameObject newpaper = Instantiate(newspaper, newsParent.transform);
            newpaper.GetComponent<Image>().sprite = Resources.Load<Sprite>("Face/" + getList[i].faceSprite);
            GameObject txt = Instantiate(getTxt, newpaper.transform);

            GetCharacterPiece(i, txt);
        }

        finishGachaBtn.SetActive(true);
    }

    public void PickUp_1()
    {
        int jewel = CSVReader.instance.playerData.jewel;
        if (jewel < 120)
        {
            UIManager.instance.WarningPopUp();
            return;
        }

        time = 1;
        OnPopUp("픽업 모집");
    }
    public void PickUp_10()
    {
        int jewel = CSVReader.instance.playerData.jewel;
        if (jewel < 1200)
        {
            UIManager.instance.WarningPopUp();
            return;
        }

        time = 10;
        OnPopUp("픽업 모집");
    }
    public void Common_1()
    {
        int jewel = CSVReader.instance.playerData.jewel;
        if (jewel < 120)
        {
            UIManager.instance.WarningPopUp();
            return;
        }

        time = 1;
        OnPopUp("상시 모집");
    }
    public void Common_10()
    {
        int jewel = CSVReader.instance.playerData.jewel;
        if (jewel < 1200)
        {
            UIManager.instance.WarningPopUp();
            return;
        }

        time = 10;
        OnPopUp("상시 모집");
    }
    public void Star3Random()
    {
        int coupon = CSVReader.instance.items[67].cnt;
        if (coupon < 1)
        {
            UIManager.instance.WarningPopUp();
            return;
        }

        time = 1;
        OnPopUp("★3 확정 모집");
    }

    void OnPopUp(string gachaName)
    {
        popUp.SetActive(true);
        message.text = $"{gachaName} {time}회를 실행하시겠습니까?";
    }

    public void PlayBtn()
    {
        popUp.SetActive(false);
        StartCoroutine(PlayGacha());
    }

    public void CancelPopUp()
    {
        popUp.SetActive(false);
    }

    public void OnPickUp()
    {
        type = GachaType.PickUp;

        pickUpPanel.SetActive(true);
        commonPanel.SetActive(false);
        star3Panel.SetActive(false);
    }
    public void OnCommon()
    {
        type = GachaType.Common;

        pickUpPanel.SetActive(false);
        commonPanel.SetActive(true);
        star3Panel.SetActive(false);
    }
    public void OnStar3()
    {
        type = GachaType.Star3;

        pickUpPanel.SetActive(false);
        commonPanel.SetActive(false);
        star3Panel.SetActive(true);
    }
    #endregion
}
