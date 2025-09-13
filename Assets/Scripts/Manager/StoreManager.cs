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

        coinItemTxt[0].text = "���� ���� : " + csv.items[61].cnt.ToString();
        coinItemTxt[1].text = "���� ���� : " + csv.items[61].cnt.ToString();
        coinItemTxt[2].text = "���� ���� : " + csv.items[62].cnt.ToString();
        coinItemTxt[3].text = "���� ���� : " + csv.items[62].cnt.ToString();
        coinItemTxt[4].text = "���� ���� : " + csv.items[63].cnt.ToString();
        coinItemTxt[5].text = "���� ���� : " + csv.items[63].cnt.ToString();
        coinItemTxt[6].text = "���� ���� : " + csv.items[65].cnt.ToString();
        coinItemTxt[7].text = "���� ���� : " + csv.items[65].cnt.ToString();


        for (int i = 0; i < csv.characters.Count; i++)
        {
            GameObject newItem = Instantiate(cardItem, cardList.transform);
            CharacterData data = csv.characters[i];
            int itemCnt = csv.items[i].cnt;

            newItem.transform.GetChild(0).GetComponent<Text>().text = data.name + "��  ����";
            newItem.transform.GetChild(2).GetComponent<Image>().sprite 
                = Resources.Load<Sprite>("Face/" + data.faceSprite);

            // �ؽ�Ʈ ������Ʈ ��������
            Text countText = newItem.transform.GetChild(4).GetComponent<Text>();
            countText.text = "���� ���� : " + csv.items[i].cnt;
            cardItemTxt.Add(countText);

            // ��ư�� ������ ���� (���ٿ��� ���� �Լ� ȣ��)
            Button btn = newItem.transform.GetChild(3).GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnCardItemClicked(data, countText));
        }

        whiteCardCnt.text = csv.items[60].cnt.ToString();
    }

    #region PopUp
    public void ItemPopUp(string itemName)
    {
        StartCoroutine(ui.SetPlayerDataToUI()); //UI ����

        //popUp.SetActive(true);
        //popUpText.text = itemName + " ���� �Ϸ�!";
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
        popUpText.text = "������ �����մϴ�!";
        StartCoroutine(ClosePopUp());
    }
    #endregion

    #region ItemButton
    // ī�� ���� ��ư ������ �� ������ ����
    private void OnCardItemClicked(CharacterData data, Text countText)
    {
        // ���� ī�� ���� üũ
        if (CSVReader.instance.items[60].cnt < 1)
        {
            WarningPopUp();
            return;
        }

        // ������ ���� ����
        CSVReader.instance.ChangeItemCount(data.id, 1);
        CSVReader.instance.ChangeItemCount(61, -1);

        // UI ������Ʈ
        countText.text = "���� ���� : " + CSVReader.instance.items[data.id - 1].cnt;
        whiteCardCnt.text = CSVReader.instance.items[60].cnt.ToString();

        // �˾�
        string itemName = data.name + "�� ���� 1��";
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

        coinItemTxt[0].text = "���� ���� : " + csv.items[61].cnt.ToString();
        coinItemTxt[1].text = "���� ���� : " + csv.items[61].cnt.ToString();

        string itemName = "����ġ ���� (��) 1��";
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

        coinItemTxt[0].text = "���� ���� : " + csv.items[61].cnt.ToString();
        coinItemTxt[1].text = "���� ���� : " + csv.items[61].cnt.ToString();

        string itemName = "����ġ ���� (��) 10��";
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

        coinItemTxt[2].text = "���� ���� : " + csv.items[62].cnt.ToString();
        coinItemTxt[3].text = "���� ���� : " + csv.items[62].cnt.ToString();

        string itemName = "����ġ ���� (��) 1��";
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

        coinItemTxt[2].text = "���� ���� : " + csv.items[62].cnt.ToString();
        coinItemTxt[3].text = "���� ���� : " + csv.items[62].cnt.ToString();

        string itemName = "����ġ ���� (��) 10��";
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

        coinItemTxt[4].text = "���� ���� : " + csv.items[63].cnt.ToString();
        coinItemTxt[5].text = "���� ���� : " + csv.items[63].cnt.ToString();

        string itemName = "����ġ ���� (��) 1��";
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

        coinItemTxt[4].text = "���� ���� : " + csv.items[63].cnt.ToString();
        coinItemTxt[5].text = "���� ���� : " + csv.items[63].cnt.ToString();

        string itemName = "����ġ ���� (��) 5��";
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

        coinItemTxt[6].text = "���� ���� : " + csv.items[65].cnt.ToString();
        coinItemTxt[7].text = "���� ���� : " + csv.items[65].cnt.ToString();

        string itemName = "��ŵ Ƽ�� 1��";
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

        coinItemTxt[6].text = "���� ���� : " + csv.items[65].cnt.ToString();
        coinItemTxt[7].text = "���� ���� : " + csv.items[65].cnt.ToString();

        string itemName = "��ŵ Ƽ�� 10��";
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
