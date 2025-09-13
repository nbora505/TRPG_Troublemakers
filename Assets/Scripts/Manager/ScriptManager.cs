using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager instance { get; private set; }
    public float charDelay = 0.01f;  // �� ���ڴ� ��� �ð�
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
            Debug.LogWarning("standingNum�� �������. �⺻�� 1�� ó��!");
            character = "1";
            UIManager.instance.lobbyCharacter.sprite = Resources.Load<Sprite>("Standing/" + character);
        }

        string raw = CSVReader.instance.scripts[int.Parse(character)-1].lobby[rand];
        string mes = raw.Replace("\uFE0F", "");

        ShowText(mes);
    }

    // �޽��� ������ ������ ȣ��
    public void ShowText(string message)
    {
        if (txt == null)
        {
            Debug.LogError("txt�� null�̴�!");
            return;
        }
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogError($"message�� �������: '{message}'");
            return;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(CoTypewrite(message));
    }

    private IEnumerator CoTypewrite(string message)
    {
        txt.text = "";        // �ؽ�Ʈ �ʱ�ȭ
        foreach (char c in message)
        {
            txt.text += c;    // �� ���ھ� �߰�
            yield return new WaitForSeconds(charDelay);
        }
    }

    #region NPCScript
    private IEnumerator NPCScript(Text t, string message)
    {
        t.text = "";        // �ؽ�Ʈ �ʱ�ȭ
        foreach (char c in message)
        {
            t.text += c;    // �� ���ھ� �߰�
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
                mes = "�ڳ׵��� ������ ��ĥ ���� �Ǿ���!";
                break;
            case 1:
                mes = "������ ������ ���ٰ�?";
                break;
            case 2:
                mes = "���� �ٷ� ��ī������ �����̶��!";
                break;
            case 3:
                mes = "��ΰ� �Ǹ��� ���Ͱ� �Ǳ� �ٶ��.";
                break;
            case 4:
                mes = "�����, �����!";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(NPCScript(QuestTxt, mes));
    }
    public void MissionNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "������ �̼� ����Ʈ�Դϴ�.";
                break;
            case 1:
                mes = "������ ���� ���� ������ ���� �ӹ��Դϴ�.";
                break;
            case 2:
                mes = "���õ� Ȱ���� ����ϰڽ��ϴ�.";
                break;
            case 3:
                mes = "�� ������ ����� �����Ͻ� �� �����ð���.";
                break;
            case 4:
                mes = "� �̼Ǻ��� �����Ͻðڽ��ϱ�?";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(NPCScript(MissionTxt, mes));
    }
    public void ClanNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "�̾�������... ���� ���� �����ϰ� ���� �ʾ�.";
                break;
            case 1:
                mes = "ģ���� ���� �༮�� ���� �� �°ų�?";
                break;
            case 2:
                mes = "���� �����ϰ� �ִ� �� �ִµ�, �� 20���.";
                break;
            case 3:
                mes = "���� �������̴�. ���ư��� ������.";
                break;
            case 4:
                mes = "���� ���� ������ ����.";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(NPCScript(ClanTxt, mes));
    }
    public void StoreNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "��.. � ������...";
                break;
            case 1:
                mes = "���Ͻô� ��ǰ�� ������ ������ �ּ���...";
                break;
            case 2:
                mes = "��... ��, �ƹ��͵� �ƴϿ���.";
                break;
            case 3:
                mes = "��... ���� ���ư��� �ʹ�.";
                break;
            case 4:
                mes = "����濡 ã�ƿ��� ������� �ʹ� ���Ƽ� ���ڿ��� ���� �Ƹ�����Ʈ��...";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(NPCScript(StoreTxt, mes));
    }
    public void StoryNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "�������� �̾߱⸦ ����ֽðڽ��ϱ�?";
                break;
            case 1:
                mes = "� ������ ���������?";
                break;
            case 2:
                mes = "�ƽ����� ������ �̾߱Ⱑ �غ�Ǿ����� �ʱ���.";
                break;
            case 3:
                mes = "...(70��° �巡���� �غ��մϴ�)";
                break;
            case 4:
                mes = "���� �̾߱⸦ �뷡�ϴ� ���������Դϴ�.";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(NPCScript(StoryTxt, mes));
    }
    public void DungeonNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "���⼭ ���� ���� �ӹ��� �� �� �־�";
                break;
            case 1:
                mes = "���� ����� �غ� �� �� �� ����";
                break;
            case 2:
                mes = "�߿�߿�߿�߿�";
                break;
            case 3:
                mes = "��... �����̰� �ƴϾ�";
                break;
            case 4:
                mes = "�ӹ��� �����Ϸ� �� �ž�?";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(NPCScript(DungeonTxt, mes));
    }
    public void ArenaNPC()
    {
        int rand = Random.Range(0, 5);
        string mes;

        switch (rand)
        {
            case 0:
                mes = "�̰��� �Ʒ�������~";
                break;
            case 1:
                mes = "���⼭ �ٸ� �÷��̾�е�� ����Ͻ� �� �־��~";
                break;
            case 2:
                mes = "���? �Ʒ����� ���� �غ� ���ΰ�����~";
                break;
            case 3:
                mes = "������ �ʿ��ϽŰ���~?";
                break;
            case 4:
                mes = "�Ʒ����� �ְ��� ���͸� ���ϴ� ���̶��ϴ�~";
                break;
            default:
                mes = "...";
                break;
        }

        StopAllCoroutines();      // ���� �ڷ�ƾ �ߴ�
        StartCoroutine(NPCScript(ArenaTxt, mes));
    }
    #endregion
}
