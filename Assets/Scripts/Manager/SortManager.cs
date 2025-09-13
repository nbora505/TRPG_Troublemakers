using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SortManager : MonoBehaviour
{
    public static SortManager instance { get; private set; }
    public UIManager UIManager;
    public CSVReader data;
    public AreaPanel areaPanel;
    public GameObject characterBtn;

    public Transform contentParent; // Scroll View의 Content 오브젝트
    public Button sortButton;
    public GameObject icon_Up;
    public GameObject icon_Down;
    public Dropdown sortDropdown;
    public bool ascending = true; // 정렬 방향 토글
    public bool isBattleList;
    public bool isLobby;

    public List<Transform> items = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        data = CSVReader.instance;
        if (gameObject.name == "CharacterPanel")
        {
            isBattleList = false;
            isLobby = false;
        }
        else if(gameObject.name == "LobbyWindow")
        {
            isBattleList = false;
            isLobby = true;
        }
        else
        {
            isBattleList = true;
            isLobby = false;
        }

        MakeCharacterBtns();
        SortItems();
    }

    public void MakeCharacterBtns()
    {
        //초기화 먼저
        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < data.characters.Count; i++)
        {
            GameObject character = Instantiate(characterBtn, contentParent);
            
            if(isBattleList)
            {
                character.GetComponent<CharacterButton>().areaPanel = areaPanel;
                character.GetComponent<Button>().onClick.AddListener
                    (character.GetComponent<CharacterButton>().CharacterSelectInBattleList);
            }
            else if (isLobby)
            {
                character.GetComponent<Button>().onClick.AddListener
                    (character.GetComponent<CharacterButton>().SetLobbyChracter);
            }
            else
            {
                character.GetComponent<Button>().onClick.AddListener(UIManager.OnCharacterSelectBtn);
            }

            CharacterButton btn = character.GetComponent<CharacterButton>();
            if (btn != null)
                btn.SetUp(data.characters[i]);

            CharacterData currentData = data.characters[i];
            Button buttonComponent = character.GetComponent<Button>();

            // 람다를 이용해 버튼 클릭 시 두 함수 호출
            buttonComponent.onClick.AddListener(() =>
            {
                UIManager.OnCharacterSelectBtn();
                UIManager.SetCharacterSelectPanel(currentData);
                UIManager.SetCharacterSkillPage(currentData);
                UIManager.SetCharacterStarPage(currentData);
            });

            if (!currentData.isGet)
                buttonComponent.interactable = false;
        }
    }

    public void SortItems()
    {
        items.Clear();

        foreach (Transform child in contentParent)
        {
            items.Add(child);
        }

        items = items.OrderBy(x => x.GetComponent<CharacterButton>().data.id).ToList();
        items = items.OrderBy(x => x.GetComponent<CharacterButton>().data.isGet).ToList();

        switch (sortDropdown.value)
        {
            case 0:
                //int 변수(star)를 기준으로 정렬
                if (ascending)
                    items = items.OrderBy(x => x.GetComponent<CharacterButton>().data.star).ToList();
                else
                    items = items.OrderByDescending(x => x.GetComponent<CharacterButton>().data.star).ToList();
                break;
            case 1:
                //float 변수(lv)를 기준으로 정렬
                if (ascending)
                    items = items.OrderBy(x => x.GetComponent<CharacterButton>().data.lv).ToList();
                else
                    items = items.OrderByDescending(x => x.GetComponent<CharacterButton>().data.lv).ToList();
                break;
            case 2:
                //가나다순으로 정렬
                if (ascending)
                    items = items.OrderBy(x => x.GetComponent<CharacterButton>().data.name).ToList();
                else
                    items = items.OrderByDescending(x => x.GetComponent<CharacterButton>().data.name).ToList();
                break;
            default:
                break;
        }

        // 재배열: 각 콘텐츠의 sibling index를 변경해 순서를 바꿉니다.
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetSiblingIndex(i);
        }

        items = items.OrderByDescending(x => x.GetComponent<CharacterButton>().data.isGet).ToList();
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetSiblingIndex(i);
        }

        ascending = !ascending; // 버튼을 누를 때마다 방향 전환 가능

        if (!ascending)
        {
            icon_Up.SetActive(true);
            icon_Down.SetActive(false);
        }
        else 
        { 
            icon_Up.SetActive(false);
            icon_Down.SetActive(true);
        }
    }
}
