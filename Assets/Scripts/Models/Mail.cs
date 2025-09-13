using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mail : MonoBehaviour
{
    public GameObject mailItem;
    GameObject ticket;
    GameObject jewel;

    void Start()
    {
        
    }

    void StartReward()
    {
        ticket = Instantiate(mailItem, transform);
        ticket.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/ticket3");
        ticket.transform.GetChild(2).GetComponent<Text>().text = "3성 캐릭터 확정 티켓";
        ticket.transform.GetChild(3).GetComponent<Text>().text = "스타트 보상입니다.";
        ticket.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(GetTicket);

        jewel = Instantiate(mailItem, transform);
        jewel.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/jewel");
        jewel.transform.GetChild(2).GetComponent<Text>().text = "쥬얼 3000개";
        jewel.transform.GetChild(3).GetComponent<Text>().text = "스타트 보상입니다.";
        jewel.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(GetJewel);
    }
    void GetTicket()
    {
        CSVReader.instance.ChangeItemCount(67, 1);
        PlayerPrefs.SetInt("StartReward", 2);
        Destroy(ticket);
    }

    void GetJewel()
    {
        CSVReader.instance.ChangePlayerStat("Jewel", 12000);
        UIManager.instance.SetPlayerDataToUI();
        PlayerPrefs.SetInt("StartReward", 2);
        Destroy(jewel);
    }
}
