using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Text nameText;
    public Text lvText;
    public Image faceImage;
    // CSV 데이터를 저장할 변수
    public CharacterData data;

    public AreaPanel areaPanel;

    GameObject[] star = new GameObject[5];

    // CSV 데이터로 UI를 초기화하는 메서드
    public void SetUp(CharacterData newData)
    {
        data = newData;
        nameText.text = newData.name;
        lvText.text = "LV." + newData.lv.ToString();

        if (newData.faceSprite != null)
        {
            faceImage.sprite = Resources.Load<Sprite>("Face/" + newData.faceSprite);
        }

        for (int i = 0; i < 5; i++)
        {
            star[i] = gameObject.transform.GetChild(i).gameObject;
            if (i + 1 > data.star)
            {
                star[i].SetActive(false);
            }
        }

    }

    public void CharacterSelectInBattleList()
    {
        //만약 전투 라인업에 이 녀석과 동일한 녀석이 등록되어 있을 경우 패스
        for (int i = 0; i < areaPanel.battleList.Count; i++)
        {
            if (areaPanel.battleList[i].gameObject == gameObject) return;
        }

        areaPanel.battleList.Add(this);
        areaPanel.battleList.Sort((a, b) => a.data.range.CompareTo(b.data.range));

        //플레이어 데이터에도 라인업 저장
        for (int i = 0; i < 5; i++)
        {
            string statName = "LineUp" + (i + 1);

            if (i < areaPanel.battleList.Count)
            {
                areaPanel.csv.ChangePlayerStat(statName, areaPanel.battleList[i].data.id);
            }
            else
            {
                areaPanel.csv.ChangePlayerStat(statName, 0);

            }
        }

        areaPanel.SetBattleListButton();
    }

    public void SetLobbyChracter()
    {
        UIManager.instance.standingNum = data.standingSprite;
        UIManager.instance.ChangeStanding();

        PlayerPrefs.SetString("Standing", data.standingSprite);
        PlayerPrefs.Save();
    }
}
