using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public GameObject coinStore;
    public GameObject arenaStore;
    public GameObject clanStore;
    public GameObject cardStore;
    public GameObject cardList;
    public GameObject cardItem;

    public Text[] coinItemTxt = new Text[8];
    public List<Text> cardItemTxt = new List<Text>();
    public Text whiteCardCnt;

    public GameObject popUp;
    public Text popUpText;

    CSVReader csv;
    UIManager ui;

    private void Start()
    {
        csv = CSVReader.instance;
        ui = UIManager.instance;

        coinItemTxt[0].text = "보유 수량 : " + csv.items[61].cnt.ToString();
        coinItemTxt[1].text = "보유 수량 : " + csv.items[61].cnt.ToString();
        coinItemTxt[2].text = "보유 수량 : " + csv.items[62].cnt.ToString();
        coinItemTxt[3].text = "보유 수량 : " + csv.items[62].cnt.ToString();
        coinItemTxt[4].text = "보유 수량 : " + csv.items[63].cnt.ToString();
        coinItemTxt[5].text = "보유 수량 : " + csv.items[63].cnt.ToString();
        coinItemTxt[6].text = "보유 수량 : " + csv.items[65].cnt.ToString();
        coinItemTxt[7].text = "보유 수량 : " + csv.items[65].cnt.ToString();


        for (int i = 0; i < csv.characters.Count; i++)
        {
            GameObject newItem = Instantiate(cardItem, cardList.transform);
            CharacterData data = csv.characters[i];
            int itemCnt = csv.items[i].cnt;

            newItem.transform.GetChild(0).GetComponent<Text>().text = data.name + "의  조각";
            newItem.transform.GetChild(2).GetComponent<Image>().sprite 
                = Resources.Load<Sprite>("Face/" + data.faceSprite);

            // 텍스트 컴포넌트 가져오기
            Text countText = newItem.transform.GetChild(4).GetComponent<Text>();
            countText.text = "보유 수량 : " + csv.items[i].cnt;
            cardItemTxt.Add(countText);

            // 버튼에 리스너 연결 (람다에서 별도 함수 호출)
            Button btn = newItem.transform.GetChild(3).GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnCardItemClicked(data, countText));
        }

        whiteCardCnt.text = csv.items[60].cnt.ToString();
    }

    #region PopUp
    public void ItemPopUp(string itemName)
    {
        StartCoroutine(ui.SetPlayerDataToUI()); //UI 세팅

        //popUp.SetActive(true);
        //popUpText.text = itemName + " 구매 완료!";
        //StartCoroutine(ClosePopUp());
    }

    IEnumerator ClosePopUp()
    {
        yield return new WaitForSeconds(1f);
        popUp.SetActive(false);
    }

    public void WarningPopUp()
    {
        popUp.SetActive(true);
        popUpText.text = "코인이 부족합니다!";
        StartCoroutine(ClosePopUp());
    }
    #endregion

    #region ItemButton
    // 카드 조각 버튼 눌렀을 때 실행할 로직
    private void OnCardItemClicked(CharacterData data, Text countText)
    {
        // 순백 카드 부족 체크
        if (CSVReader.instance.items[60].cnt < 1)
        {
            WarningPopUp();
            return;
        }

        // 아이템 수량 변경
        CSVReader.instance.ChangeItemCount(data.id, 1);
        CSVReader.instance.ChangeItemCount(61, -1);

        // UI 업데이트
        countText.text = "보유 수량 : " + CSVReader.instance.items[data.id - 1].cnt;
        whiteCardCnt.text = CSVReader.instance.items[60].cnt.ToString();

        // 팝업
        string itemName = data.name + "의 조각 1개";
        ItemPopUp(itemName);
    }
    public void expS()
    {
        if(CSVReader.instance.playerData.coin < 500)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -500);
        CSVReader.instance.ChangeItemCount(62, 1);

        coinItemTxt[0].text = "보유 수량 : " + csv.items[61].cnt.ToString();
        coinItemTxt[1].text = "보유 수량 : " + csv.items[61].cnt.ToString();

        string itemName = "경험치 물약 (소) 1개";
        ItemPopUp(itemName);
    }
    public void expS_10()   
    {
        if (CSVReader.instance.playerData.coin < 5000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -5000);
        CSVReader.instance.ChangeItemCount(62, 10);

        coinItemTxt[0].text = "보유 수량 : " + csv.items[61].cnt.ToString();
        coinItemTxt[1].text = "보유 수량 : " + csv.items[61].cnt.ToString();

        string itemName = "경험치 물약 (소) 10개";
        ItemPopUp(itemName);
    }
    public void expM()
    {
        if (CSVReader.instance.playerData.coin < 1000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -1000);
        CSVReader.instance.ChangeItemCount(63, 1);

        coinItemTxt[2].text = "보유 수량 : " + csv.items[62].cnt.ToString();
        coinItemTxt[3].text = "보유 수량 : " + csv.items[62].cnt.ToString();

        string itemName = "경험치 물약 (중) 1개";
        ItemPopUp(itemName);
    }
    public void expM_10()
    {
        if (CSVReader.instance.playerData.coin < 10000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -10000);
        CSVReader.instance.ChangeItemCount(63, 10);

        coinItemTxt[2].text = "보유 수량 : " + csv.items[62].cnt.ToString();
        coinItemTxt[3].text = "보유 수량 : " + csv.items[62].cnt.ToString();

        string itemName = "경험치 물약 (중) 10개";
        ItemPopUp(itemName);
    }
    public void expL()
    {
        if (CSVReader.instance.playerData.coin < 30000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -30000);
        CSVReader.instance.ChangeItemCount(64, 1);

        coinItemTxt[4].text = "보유 수량 : " + csv.items[63].cnt.ToString();
        coinItemTxt[5].text = "보유 수량 : " + csv.items[63].cnt.ToString();

        string itemName = "경험치 물약 (대) 1개";
        ItemPopUp(itemName);
    }
    public void expL_5()
    {
        if (CSVReader.instance.playerData.coin < 150000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -150000);
        CSVReader.instance.ChangeItemCount(64, 5);

        coinItemTxt[4].text = "보유 수량 : " + csv.items[63].cnt.ToString();
        coinItemTxt[5].text = "보유 수량 : " + csv.items[63].cnt.ToString();

        string itemName = "경험치 물약 (대) 5개";
        ItemPopUp(itemName);
    }
    public void ticket()
    {
        if (CSVReader.instance.playerData.coin < 1200)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -1200);
        CSVReader.instance.ChangeItemCount(66, 1);

        coinItemTxt[6].text = "보유 수량 : " + csv.items[65].cnt.ToString();
        coinItemTxt[7].text = "보유 수량 : " + csv.items[65].cnt.ToString();

        string itemName = "스킵 티켓 1개";
        ItemPopUp(itemName);
    }
    public void ticket_10()
    {
        if (CSVReader.instance.playerData.coin < 12000)
        {
            WarningPopUp();
            return;
        }

        CSVReader.instance.ChangePlayerStat("Coin", -12000);
        CSVReader.instance.ChangeItemCount(66, 10);

        coinItemTxt[6].text = "보유 수량 : " + csv.items[65].cnt.ToString();
        coinItemTxt[7].text = "보유 수량 : " + csv.items[65].cnt.ToString();

        string itemName = "스킵 티켓 10개";
        ItemPopUp(itemName);
    }
    #endregion

    #region StoreButton

    public void OnCoinStore()
    {
        coinStore.SetActive(true);
        arenaStore.SetActive(false);
        clanStore.SetActive(false);
        cardStore.SetActive(false);
    }
    public void OnArenaStore()
    {
        coinStore.SetActive(false);
        arenaStore.SetActive(true);
        clanStore.SetActive(false);
        cardStore.SetActive(false);
    }
    public void OnClanStore()
    {
        coinStore.SetActive(false);
        arenaStore.SetActive(false);
        clanStore.SetActive(true);
        cardStore.SetActive(false);
    }
    public void OnCardStore()
    {
        coinStore.SetActive(false);
        arenaStore.SetActive(false);
        clanStore.SetActive(false);
        cardStore.SetActive(true);
    }
    #endregion
}
