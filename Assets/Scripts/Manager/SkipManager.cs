using UnityEngine;
using UnityEngine.UI;

public class SkipManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject skipPanel;
    public Text ticketCnt;
    public Text useTicketCnt;
    public GameObject getItemList;
    public GameObject barrier;

    [Header("Prefab")]
    public GameObject getItem;
    public StageData stage;
    public Sprite jewelSprite;
    public Sprite coinSprite;
    public Sprite ticketSprite;
    public Sprite couponSprite;
    public Sprite poS;
    public Sprite poM;
    public Sprite poL;
    public Sprite poXL;

    public int myTicketCnt;
    public int curTicketCnt = 0;

    GameManager gm;
    CSVReader csv;

    private void OnEnable()
    {
        gm = GameManager.instance;
        csv = CSVReader.instance;

        myTicketCnt = csv.items[65].cnt;

        //�ʱ�ȭ ����
        foreach (Transform child in getItemList.transform)
        {
            Destroy(child.gameObject);
        }

        ticketCnt.text = $"���� ���� : {myTicketCnt}";
        useTicketCnt.text = curTicketCnt.ToString();

        if(gm.curStage[0].star < 1) barrier.SetActive(true);
        else barrier.SetActive(false);
    }

    public void UpTicketCnt()
    {
        if (curTicketCnt + 1> myTicketCnt) return;

        curTicketCnt++;
        useTicketCnt.text = curTicketCnt.ToString();
    }
    public void DownTicketCnt()
    {
        if (curTicketCnt - 1 < 0) return;

        curTicketCnt--;
        useTicketCnt.text = curTicketCnt.ToString();
    }

    public void UseTicketBtn()
    {
        if(curTicketCnt<1) return;
        if (curTicketCnt * 5 > PlayerPrefs.GetInt("Energy")) return;

        skipPanel.SetActive(true);

        foreach (Transform child in getItemList.transform)
        {
            Destroy(child.gameObject);
        }

        csv.ChangeItemCount(66, curTicketCnt * -1);

        GetReward(curTicketCnt);
        
        myTicketCnt = csv.items[65].cnt;
        ticketCnt.text = $"���� ���� : {myTicketCnt}";
        StartCoroutine(UIManager.instance.SetPlayerDataToUI());
    }

    void GetReward(int j)
    {
        ItemManager im = gm.gameObject.GetComponent<ItemManager>();
        int itemCnt = 4;


        int s, m, l, xl, coin;
        im.GeneratePotions(stage.stageNum / 10000, out s, out m, out l, out xl);
        coin = im.GetCoins(stage.stageNum / 10000);

        s *= j; m *= j; l *= j; xl *= j; coin *= j;
        
        //������ �� �߰��ϱ�
        csv.ChangePlayerStat("Jewel", 50 * j);
        csv.ChangePlayerStat("Coin", coin);
        csv.ChangeItemCount(62, s);
        csv.ChangeItemCount(63, m);
        csv.ChangeItemCount(64, l);
        csv.ChangeItemCount(65, xl);
        csv.ChangeItemCount(66, j);

        //Ŭ�����гο� ȹ�� ������ �����ֱ�
        if (m > 0) itemCnt++; if (l > 0) itemCnt++; if (xl > 0) itemCnt++;

        for (int i = 0; i < itemCnt; i++)
        {
            GameObject newItem = Instantiate(getItem, getItemList.transform);
            Image icon = newItem.transform.GetChild(0).GetComponent<Image>();
            Text name = newItem.transform.GetChild(1).GetComponent<Text>();

            switch (i)
            {
                case 0:
                    icon.sprite = jewelSprite;
                    name.text = "���� x" + 50 * j;
                    break;
                case 1:
                    icon.sprite = coinSprite;
                    name.text = "��� x" + coin.ToString();
                    break;
                case 2:
                    icon.sprite = ticketSprite;
                    name.text = "��ŵ Ƽ�� x"+j;
                    break;
                case 3:
                    icon.sprite = poS;
                    name.text = "����ġ ����(��) x" + s.ToString();
                    break;
                case 4:
                    icon.sprite = poS;
                    name.text = "����ġ ����(��) x" + m.ToString();
                    break;
                case 5:
                    icon.sprite = poS;
                    name.text = "����ġ ����(��) x" + l.ToString();
                    break;
                case 6:
                    icon.sprite = poS;
                    name.text = "����ġ ����(Ư��) x" + xl.ToString();
                    break;
                default:
                    break;
            }
        }

        int energy = PlayerPrefs.GetInt("Energy");
        PlayerPrefs.SetInt("Energy", energy - (5 * j));
        PlayerPrefs.Save();
    }

    public void OkayBtn()
    {
        skipPanel.SetActive(false);
    }
}
