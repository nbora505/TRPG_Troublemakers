using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaContent : MonoBehaviour
{
    public AreaPanel areaPanel;
    public Text areaNumber;
    public Text areaName;

    public List<Button> areaBtn = new List<Button>();
    Button curBtn;

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            areaBtn.Add(gameObject.transform.GetChild(i).GetComponent<Button>());
        }
       
    }
    void Update()
    {
        float x = transform.position.x;

        if(x >= -430)
        {
            areaNumber.text = "Area 1";
            areaName.text = "스페로 헌터 학교";
            curBtn = areaBtn[0];
        }
        else if(x < -430 &&  x >= -1330)
        {
            areaNumber.text = "Area 2";
            areaName.text = "기계인형의 게이트";
            curBtn = areaBtn[1];
        }
        else if (x < -1330 && x >= -2230)
        {
            areaNumber.text = "Area 3";
            areaName.text = "N-11 설산";
            curBtn = areaBtn[2];
        }
        else if (x < -2230 && x >= -3130)
        {
            areaNumber.text = "Area 4";
            areaName.text = "망령들의 연구소";
            curBtn = areaBtn[3];
        }
        else if (x < -3130 && x >= -4030)
        {
            areaNumber.text = "Area 5";
            areaName.text = "메마른 왕국";
            curBtn = areaBtn[4];
        }
        else if (x < -4030 && x >= -4930)
        {
            areaNumber.text = "Area 6";
            areaName.text = "별과 피라미드";
            curBtn = areaBtn[5];
        }
        else if (x < -4930 && x >= -5830)
        {
            areaNumber.text = "Area 7";
            areaName.text = "푸른 늑대의 교단";
            curBtn = areaBtn[6];
        }
        else if (x < -5830 && x >= -6730)
        {
            areaNumber.text = "Area 8";
            areaName.text = "영원한 게이트";
            curBtn = areaBtn[7];
        }
        else if (x < -6730 && x >= -7630)
        {
            areaNumber.text = "Area 9";
            areaName.text = "재앙이 내린 도시";
            curBtn = areaBtn[8];
        }
        else
        {
            areaNumber.text = "Area 10";
            areaName.text = "최종 결전";
            curBtn = areaBtn[9];
        }

        for (int i = 0; i < areaBtn.Count; i++)
        {
            if (areaBtn[i] != curBtn)
            {
                areaBtn[i].interactable = false;
            }
            else
            {
                areaBtn[i].interactable = true;
                areaPanel.areaNum = i + 1;
            }
        }
    }
}
