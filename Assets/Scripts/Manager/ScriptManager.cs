using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager instance { get; private set; }
    public float charDelay = 0.01f;  // 한 글자당 대기 시간
    TextMeshProUGUI txt;

    public Text QuestTxt;
    public Text MissionTxt;
    public Text ClanTxt;
    public Text StoreTxt;
    public Text StoryTxt;
    public Text DungeonTxt;
    public Text ArenaTxt;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        txt = GetComponent<TextMeshProUGUI>();

        SetLobbyMessage();
    }

    public void SetLobbyMessage()
    {

        string character = UIManager.instance.standingNum;
        int rand = Random.Range(0, 5);

        if (string.IsNullOrWhiteSpace(character))
        {
            Debug.LogWarning("standingNum이 비어있음. 기본값 1로 처리!");
            character = "1";
            UIManager.instance.lobbyCharacter.sprite = Resources.Load<Sprite>("Standing/" + character);
        }

        string raw = CSVReader.instance.scripts[int.Parse(character)-1].lobby[rand];
        string mes = raw.Replace("\uFE0F", "");

        ShowText(mes);
    }

    // 메시지 보여줄 때마다 호출
    public void ShowText(string message)
    {
        if (txt == null)
        {
            Debug.LogError("txt가 null이다!");
            return;
        }
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogError($"message가 비어있음: '{message}'");
            return;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(CoTypewrite(message));
    }

    private IEnumerator CoTypewrite(string message)
    {
        txt.text = "";        // 텍스트 초기화
        foreach (char c in message)
        {
            txt.text += c;    // 한 글자씩 추가
            yield return new WaitForSeconds(charDelay);
        }
    }

    #region NPCScript
    private IEnumerator NPCScript(Text t, string message)
    {
        t.text = "";        // 텍스트 초기화
        foreach (char c in message)
        {
            t.text += c;    // 한 글자씩 추가
            yield return new WaitForSeconds(charDelay);
        }
    }

    public void QuestNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "자네들의 모험을 펼칠 때가 되었네!";
                break;
            case 1:
                mes = "오늘은 무엇을 할텐가?";
                break;
            case 2:
                mes = "내가 바로 아카데미의 교장이라네!";
                break;
            case 3:
                mes = "모두가 훌륭한 헌터가 되길 바라네.";
                break;
            case 4:
                mes = "스페로, 스페라!";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(NPCScript(QuestTxt, mes));
    }
    public void MissionNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "오늘의 미션 리스트입니다.";
                break;
            case 1:
                mes = "스케줄 관리 또한 유능한 비서의 임무입니다.";
                break;
            case 2:
                mes = "오늘도 활약을 기대하겠습니다.";
                break;
            case 3:
                mes = "이 정도는 충분히 수행하실 수 있으시겠죠.";
                break;
            case 4:
                mes = "어떤 미션부터 수행하시겠습니까?";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(NPCScript(MissionTxt, mes));
    }
    public void ClanNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "미안하지만... 길드는 아직 영업하고 있지 않아.";
                break;
            case 1:
                mes = "친구도 없는 녀석이 여긴 왜 온거냐?";
                break;
            case 2:
                mes = "뭔가 오해하고 있는 게 있는데, 난 20대다.";
                break;
            case 3:
                mes = "여긴 공사중이다. 돌아가라 꼬맹이.";
                break;
            case 4:
                mes = "역시 나는 비율이 좋군.";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(NPCScript(ClanTxt, mes));
    }
    public void StoreNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "어.. 어서 오세요...";
                break;
            case 1:
                mes = "원하시는 상품이 있으면 말씀해 주세요...";
                break;
            case 2:
                mes = "할... 아, 아무것도 아니에요.";
                break;
            case 3:
                mes = "아... 집에 돌아가고 싶다.";
                break;
            case 4:
                mes = "자취방에 찾아오는 사람들이 너무 많아서 팔자에도 없는 아르바이트를...";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(NPCScript(StoreTxt, mes));
    }
    public void StoryNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "여러분의 이야기를 들려주시겠습니까?";
                break;
            case 1:
                mes = "어떤 모험이 펼쳐질까요?";
                break;
            case 2:
                mes = "아쉽지만 아직은 이야기가 준비되어있지 않군요.";
                break;
            case 3:
                mes = "...(70번째 드래곤을 준비합니다)";
                break;
            case 4:
                mes = "저는 이야기를 노래하는 음유시인입니다.";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(NPCScript(StoryTxt, mes));
    }
    public void DungeonNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "여기서 던전 수색 임무를 할 수 있어";
                break;
            case 1:
                mes = "아직 여기는 준비 안 된 것 같네";
                break;
            case 2:
                mes = "삐용삐용삐용삐용";
                break;
            case 3:
                mes = "난... 꼬맹이가 아니야";
                break;
            case 4:
                mes = "임무를 수행하러 온 거야?";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(NPCScript(DungeonTxt, mes));
    }
    public void ArenaNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "이곳은 아레나에요~";
                break;
            case 1:
                mes = "여기서 다른 플레이어분들과 대결하실 수 있어요~";
                break;
            case 2:
                mes = "어라? 아레나는 아직 준비 중인가봐요~";
                break;
            case 3:
                mes = "도움이 필요하신가요~?";
                break;
            case 4:
                mes = "아레나는 최고의 헌터를 정하는 곳이랍니다~";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // 이전 코루틴 중단
        StartCoroutine(NPCScript(ArenaTxt, mes));
    }
    #endregion
}
