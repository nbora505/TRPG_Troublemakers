using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using System.Linq;

public class CSVReader : MonoBehaviour
{
    // ĳ���� ������ ������ ����Ʈ (����� ���� Ŭ���� ��� ����)
    public static CSVReader instance { get; private set; }
    public PlayerData playerData = new PlayerData();
    public List<CharacterData> characters = new List<CharacterData>();
    public List<CharacterData> enemys = new List<CharacterData>();
    public List<StageData> stages = new List<StageData>();
    public List<SkillData> skills = new List<SkillData>();
    public List<ItemData> items = new List<ItemData>();
    public List<ScriptData> scripts = new List<ScriptData>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // ù ���� ���� �ı� ����
        }
        else if (instance != this)
        {
            Destroy(gameObject);            // �̹� ������ �ڱ� �ڽ� �ı�
            return;
        }

        ParseCharacterCSV();
        ParsePlayerCSV();
        ParseEnemyCSV();
        ParseStageCSV();
        ParseSkillCSV();
        ParseItemCSV();
        ParseScriptCSV();
    }

    public void ParseCharacterCSV()
    {
        string csvFileName = "TrpgCharacters.csv";

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        string data = File.ReadAllText(filePath);

        // �ٹٲ�(\n) �������� �и��Ͽ� �� ���� ����
        string[] lines = data.Split('\n');

        // ù ���� ����� ��� �ǳʶٱ�
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // �� �� üũ

            // ��ǥ�� �и�
            string[] columns = lines[i].Split(',');

            CharacterData newCharacter = new CharacterData();

            newCharacter.id = int.Parse(columns[0]);
            newCharacter.name = columns[1];
            newCharacter.star = int.Parse(columns[2]);

            newCharacter.lv = int.Parse(columns[3]);
            newCharacter.exp = float.Parse(columns[4]);

            newCharacter.baseHp = float.Parse(columns[5]);
            newCharacter.baseAtk = float.Parse(columns[6]);
            newCharacter.baseMAtk = float.Parse(columns[7]);
            newCharacter.baseDef = float.Parse(columns[8]);
            newCharacter.baseMDef = float.Parse(columns[9]);
            newCharacter.baseCri = float.Parse(columns[10]);
            newCharacter.baseMiss = float.Parse(columns[11]);

            newCharacter.range = float.Parse(columns[12]);

            for (int j = 0; j < 6; j++)
                newCharacter.pattern[j] = int.Parse(columns[13 + j]);
            for (int j = 0; j < 3; j++)
                newCharacter.skill[j] = int.Parse(columns[19 + j]);
            newCharacter.exSkill = int.Parse(columns[22]);

            newCharacter.faceSprite = columns[23];
            newCharacter.standingSprite = columns[24];
            newCharacter.sdSprite = columns[25];

            bool parsedGet = false;
            if (columns.Length > 26 && bool.TryParse(columns[26].Trim(), out parsedGet))
            {
                newCharacter.isGet = parsedGet;
            }
            else
            {
                // �� ���ڿ� �Ǵ� "1"/"0" ���� ��� ó���ϰ� ������ ���⿡ �߰� ����
                newCharacter.isGet = false; // �⺻��
            }

            CalculateCurrentStats(newCharacter);
            characters.Add(newCharacter);
            //Debug.Log(newCharacter.name);
        }
    }
    public void ParsePlayerCSV()
    {
        string csvFileName = "PlayerInfo.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV ������ ã�� �� �����ϴ�: " + filePath);
            return;
        }

        // CSV ������ ��� ���� �о�ɴϴ�.
        string[] lines = File.ReadAllLines(filePath);

        // ���ο� �÷��̾� ������ ��ü�� �����մϴ�.
        PlayerData newPlayerData = new PlayerData();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // �� ���� �޸�(,)�� �и��մϴ�.
            string[] columns = line.Split(',');
            // ù ��° �׸��� Ű, �� ��° �׸��� ��(������ ���)�Դϴ�.
            string key = columns[0].Trim();
            string value = columns.Length >= 2 ? columns[1].Trim() : "";

            // Ű�� ���� �÷��̾� �������� �ʵ带 �����մϴ�.
            switch (key)
            {
                case "Name":
                    newPlayerData.name = value;
                    break;
                case "ID":
                    newPlayerData.id = value;
                    break;
                case "PW":
                    newPlayerData.pw = value;
                    break;
                case "LV":
                    if (int.TryParse(value, out int lv))
                        newPlayerData.lv = lv;
                    else
                        Debug.LogWarning("LV ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "Exp":
                    if (int.TryParse(value, out int exp))
                        newPlayerData.exp = exp;
                    else
                        Debug.LogWarning("Exp ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "Energy":
                    if (int.TryParse(value, out int energy))
                        newPlayerData.energy = energy;
                    else
                        Debug.LogWarning("Energy ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "Coin":
                    if (int.TryParse(value, out int coin))
                        newPlayerData.coin = coin;
                    else
                        Debug.LogWarning("Coin ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "Jewel":
                    if (int.TryParse(value, out int jewel))
                        newPlayerData.jewel = jewel;
                    else
                        Debug.LogWarning("Jewel ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "isFirstTime":
                    if (bool.TryParse(value, out bool isFirstTime))
                        newPlayerData.isFirstTime = isFirstTime;
                    else
                        Debug.LogWarning("isFirstTime ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "LineUp1":
                    if (int.TryParse(value, out int lineup1))
                        newPlayerData.lineUp[0] = lineup1;
                    else
                        Debug.LogWarning("LineUp1 ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "LineUp2":
                    if (int.TryParse(value, out int lineup2))
                        newPlayerData.lineUp[1] = lineup2;
                    else
                        Debug.LogWarning("LineUp2 ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "LineUp3":
                    if (int.TryParse(value, out int lineup3))
                        newPlayerData.lineUp[2] = lineup3;
                    else
                        Debug.LogWarning("LineUp3 ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "LineUp4":
                    if (int.TryParse(value, out int lineup4))
                        newPlayerData.lineUp[3] = lineup4;
                    else
                        Debug.LogWarning("LineUp4 ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                case "LineUp5":
                    if (int.TryParse(value, out int lineup5))
                        newPlayerData.lineUp[4] = lineup5;
                    else
                        Debug.LogWarning("LineUp5 ���� �Ľ��� �� �����ϴ�: " + value);
                    break;
                default:
                    Debug.LogWarning("�� �� ���� �ʵ��: " + key);
                    break;
            }
            //Debug.LogWarning(value);
        }

        playerData = newPlayerData;
        Debug.Log("�÷��̾� ������ �ε� ����: " + newPlayerData.name);
    }
    public void ParseEnemyCSV()
    {
        string csvFileName = "EnemyInfo.csv";

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        string data = File.ReadAllText(filePath);

        // �ٹٲ�(\n) �������� �и��Ͽ� �� ���� ����
        string[] lines = data.Split('\n');

        // ù ���� ����� ��� �ǳʶٱ�
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // �� �� üũ

            // ��ǥ�� �и�
            string[] columns = lines[i].Split(',');

            CharacterData newEnemy = new CharacterData();

            newEnemy.id = int.Parse(columns[0]);
            newEnemy.name = columns[1];

            newEnemy.hp = float.Parse(columns[2]);
            newEnemy.atk = float.Parse(columns[3]);
            newEnemy.mAtk = float.Parse(columns[4]);
            newEnemy.def = float.Parse(columns[5]);
            newEnemy.mDef = float.Parse(columns[6]);
            newEnemy.cri = float.Parse(columns[7]);
            newEnemy.miss = float.Parse(columns[8]);

            for (int j = 0; j < 6; j++)
                newEnemy.pattern[j] = int.Parse(columns[9 + j]);
            for (int j = 0; j < 3; j++)
                newEnemy.skill[j] = int.Parse(columns[15 + j]);
            newEnemy.exSkill = int.Parse(columns[18]);

            enemys.Add(newEnemy);
            //Debug.Log(newEnemy.name);
        }
    }
    public void ParseStageCSV()
    {
        string csvFileName = "StageInfo.csv";

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        string data = File.ReadAllText(filePath);

        // �ٹٲ�(\n) �������� �и��Ͽ� �� ���� ����
        string[] lines = data.Split('\n');

        // ù ���� ����� ��� �ǳʶٱ�
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // �� �� üũ

            // ��ǥ�� �и�
            string[] columns = lines[i].Split(',');

            StageData newStage = new StageData();

            newStage.stageNum = int.Parse(columns[0]);
            newStage.stageName = columns[1];

            for (int j = 0; j < 5; j++)
            {
                if (!string.IsNullOrEmpty(columns[2 + j]))
                    newStage.enemyId[j] = int.Parse(columns[2 + j]);
            }
            for (int j = 0; j < 5; j++)
            {
                if (!string.IsNullOrEmpty(columns[7 + j]))
                    newStage.enemyPos[j] = float.Parse(columns[7 + j]);
            }
            newStage.stageBG = columns[12];

            newStage.star = int.Parse(columns[13]);

            stages.Add(newStage);
        }
    }
    public void ParseSkillCSV()
    {
        string csvFileName = "SkillInfo.csv";

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        string data = File.ReadAllText(filePath);

        // �ٹٲ�(\n) �������� �и��Ͽ� �� ���� ����
        string[] lines = data.Split('\n');

        // ù ���� ����� ��� �ǳʶٱ�
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // �� �� üũ

            // ��ǥ�� �и�
            string[] columns = lines[i].Split(',');

            SkillData newSkill = new SkillData();

            newSkill.id = int.Parse(columns[0]);
            newSkill.name = columns[1];
            newSkill.description = columns[2];
            newSkill.lv = int.Parse(columns[3]);

            newSkill.typeA = columns[4];
            newSkill.targetA = columns[5];
            newSkill.sizeA = columns[6];
            newSkill.timeA = float.Parse(columns[7]);

            newSkill.typeB = columns[8];
            newSkill.targetB = columns[9];
            newSkill.sizeB = columns[10];
            newSkill.timeB = float.Parse(columns[11]);

            newSkill.castEffect = columns[12];
            newSkill.hitEffect = columns[13];

            skills.Add(newSkill);
        }
    }
    public void ParseItemCSV()
    {
        string csvFileName = "Items.csv";

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        string data = File.ReadAllText(filePath);

        // �ٹٲ�(\n) �������� �и��Ͽ� �� ���� ����
        string[] lines = data.Split('\n');

        // ù ���� ����� ��� �ǳʶٱ�
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // �� �� üũ

            // ��ǥ�� �и�
            string[] columns = lines[i].Split(',');

            ItemData newItem = new ItemData();

            newItem.id = int.Parse(columns[0]);
            newItem.name = columns[1];
            newItem.script = columns[2];
            newItem.cnt = int.Parse(columns[3]);

            items.Add(newItem);
        }
    }
    public void ParseScriptCSV()
    {
        string csvFileName = "Script.csv";

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        string data = File.ReadAllText(filePath);

        // �ٹٲ�(\n) �������� �и��Ͽ� �� ���� ����
        string[] lines = data.Split('\n');

        // ù ���� ����� ��� �ǳʶٱ�
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // �� �� üũ

            // ��ǥ�� �и�
            string[] columns = lines[i].Split(',');

            ScriptData newScript = new ScriptData();

            newScript.id = int.Parse(columns[0]);
            newScript.name = columns[1];

            for (int j = 0; j < 5; j++)
            {
                newScript.lobby[j] = columns[2+j];
            }
            newScript.win = columns[7];
            newScript.defeat = columns[8];
            newScript.get = columns[9];

            scripts.Add(newScript);
        }
    }

    public void ChangePlayerStat(string stat, int amount)
    {
        string csvFileName = "PlayerInfo.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV ������ ã�� �� �����ϴ�: " + filePath);
            return;
        }

        // CSV ������ ��� ���� ����Ʈ ���·� �о�ɴϴ�.
        List<string> lines = new List<string>(File.ReadAllLines(filePath));
        bool statUpdated = false;

        // �� ���� ��ȸ�ϸ� "stat" Ű�� ã���ϴ�.
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith(stat))
            {
                string[] columns = lines[i].Split(',');
                if (columns.Length >= 2)
                {
                    if (int.TryParse(columns[1].Trim(), out int currentStat))
                    {
                        currentStat += amount;
                        // ���ο� ������ ������Ʈ�� �� �籸��
                        lines[i] = stat + "," + currentStat.ToString();
                        // �޸� ���� playerData�� ������Ʈ
                        switch (stat)
                        {
                            case "LV":
                                playerData.lv = currentStat;
                                break;
                            case "Exp":
                                playerData.exp = currentStat; 
                                break;
                            case "Energy":
                                playerData.energy = currentStat;
                                break;
                            case "Coin":
                                playerData.coin = currentStat;
                                break;
                            case "Jewel":
                                playerData.jewel = currentStat;
                                break;
                            case "LineUp1":
                                currentStat = amount;
                                lines[i] = stat + "," + currentStat.ToString();
                                playerData.lineUp[0] = currentStat;
                                break;
                            case "LineUp2":
                                currentStat = amount;
                                lines[i] = stat + "," + currentStat.ToString();
                                playerData.lineUp[1] = currentStat;
                                break;
                            case "LineUp3":
                                currentStat = amount;
                                lines[i] = stat + "," + currentStat.ToString();
                                playerData.lineUp[2] = currentStat;
                                break;
                            case "LineUp4":
                                currentStat = amount;
                                lines[i] = stat + "," + currentStat.ToString();
                                playerData.lineUp[3] = currentStat;
                                break;
                            case "LineUp5":
                                currentStat = amount;
                                lines[i] = stat + "," + currentStat.ToString();
                                playerData.lineUp[4] = currentStat;
                                break;
                            default:
                                break;
                        }

                        Debug.Log(stat + "������Ʈ �Ϸ�: " + currentStat);
                        statUpdated = true;
                    }
                    else
                    {
                        Debug.LogWarning("���� �Ľ��� �� �����ϴ�: " + columns[1]);
                    }
                }
                break;
            }
        }

        if (!statUpdated)
        {
            Debug.LogWarning("Ű�� CSV ���Ͽ��� ã�� �� �����ϴ�.");
            return;
        }

        // ������Ʈ�� ������ CSV ���Ͽ� �ٽ� ���ϴ�.
        try
        {
            File.WriteAllLines(filePath, lines.ToArray());
            Debug.Log("CSV ������ ���������� ������Ʈ�Ǿ����ϴ�.");
        }
        catch (IOException ex)
        {
            Debug.LogError("CSV ���� ���� �� ���� �߻�: " + ex.Message);
        }
    }

    public void ChangeItemCount(int itemId, int amount)
    {
        string csvFileName = "Items.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV ������ ã�� �� �����ϴ�: {filePath}");
            return;
        }

        // ���� ��ü�� �о ���� ���� ����Ʈ�� ��ȯ
        List<string> lines = new List<string>(File.ReadAllLines(filePath));
        bool updated = false;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cols = line.Split(',');
            // ù ���� ������ ID
            if (int.TryParse(cols[0], out int id) && id == itemId)
            {
                // 4��° ��(cnt) �Ľ�
                if (int.TryParse(cols[3], out int cnt))
                {
                    cnt += amount;
                    if (cnt < 0) cnt = 0;  // ���� ����
                    cols[3] = cnt.ToString();

                    // ������Ʈ�� �������� ��ü
                    lines[i] = string.Join(",", cols);
                    updated = true;

                    // �޸𸮻��� items ����Ʈ�� ����ȭ
                    ItemData item = items.Find(x => x.id == itemId);
                    if (item != null)
                    {
                        item.cnt = cnt;
                        Debug.Log($"[CSVReader] ������ \"{item.name}\" ���� ������Ʈ: {cnt}");
                    }
                    else
                    {
                        Debug.LogWarning($"[CSVReader] �޸𸮻��� items���� ID {itemId}�� ã�� �� �����ϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning($"[CSVReader] Items.csv�� cnt �Ľ� ����: {cols[3]}");
                }
                break;
            }
        }

        if (!updated)
        {
            Debug.LogWarning($"[CSVReader] Items.csv���� ID {itemId}�� ã�� �� �����ϴ�.");
            return;
        }

        // ����� ������ ���Ͽ� ���
        try
        {
            File.WriteAllLines(filePath, lines.ToArray());
            Debug.Log("Items.csv ������ ���������� ������Ʈ�Ǿ����ϴ�.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"[CSVReader] CSV ���� ���� �� ���� �߻�: {ex.Message}.");
        }
    }

    // CSV ���� stat��Į�� �ε����� �̸� �����صΰ�, �� ���� ó���Ѵ�.
    static readonly Dictionary<string, int> charFieldMap = new Dictionary<string, int>
{
    {"lv",       3},  {"exp",      4},  {"hp",      5},  {"atk",     6},
    {"mAtk",     7},  {"def",      8},  {"mDef",    9},  {"cri",    10},
    {"miss",    11},  {"range",   12},  {"star",    2},  {"isGet",  26}
};

    public void ChangeCharacterStat(int characterId, string stat, float amount)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "TrpgCharacters.csv");
        if (!File.Exists(path)) { Debug.LogError($"CSV �� ã����: {path}"); return; }

        var lines = File.ReadAllLines(path).ToList();
        for (int i = 1; i < lines.Count; i++)
        {
            var cols = lines[i].Split(',');
            if (!int.TryParse(cols[0], out int id) || id != characterId) continue;

            if (!charFieldMap.TryGetValue(stat, out int idx))
            {
                Debug.LogWarning($"�� �� ���� �ʵ�: {stat}"); return;
            }

            // ���� �ʵ常 ó�� (star, isGet�� float�� �ٲ� �� ����)
            float cur = float.Parse(cols[idx]);
            float updated = cur + amount;
            cols[idx] = updated.ToString();

            // �޸� ����ȭ
            var chr = characters.Find(c => c.id == characterId);
            if (chr != null)
            {
                // reflection ��� switch�� ���� ����
                switch (stat)
                {
                    case "lv": chr.lv = (int)updated; break;
                    case "exp": chr.exp = updated; break;
                    case "hp": chr.hp = updated; break;
                    case "atk": chr.atk = updated; break;
                    case "mAtk": chr.mAtk = updated; break;
                    case "def": chr.def = updated; break;
                    case "mDef": chr.mDef = updated; break;
                    case "cri": chr.cri = updated; break;
                    case "miss": chr.miss = updated; break;
                    case "range": chr.range = updated; break;
                    case "star": chr.star = (int)updated; break;
                    case "isGet": chr.isGet = updated > 0; break;
                }
            }

            lines[i] = string.Join(",", cols);
            break;
        }
        File.WriteAllLines(path, lines);
        Debug.Log($"{characterId}�� {stat}��(��) {amount}��ŭ ���� �Ϸ�.");
    }

    // ������ ĳ������ isGet �÷���(true/false)�� ����ϵ� ����
    public void ChangeCharacterIsGet(int characterId, bool newIsGet)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "TrpgCharacters.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV �� ã�ڴ� ��: {path}");
            return;
        }

        var lines = File.ReadAllLines(path).ToList();
        bool done = false;

        for (int i = 1; i < lines.Count; i++)  // 0��°�� ���
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var cols = lines[i].Split(',');
            if (!int.TryParse(cols[0], out int id) || id != characterId) continue;

            // CSV �÷� 26�� isGet ��ġ
            cols[26] = newIsGet.ToString().ToLower();
            lines[i] = string.Join(",", cols);

            // �޸� ����ȭ
            var chr = characters.Find(c => c.id == characterId);
            if (chr != null) chr.isGet = newIsGet;

            done = true;
            break;
        }

        if (!done)
        {
            Debug.LogWarning($"ID {characterId} ĳ���� �� ã�ڴ� ��.");
            return;
        }

        try
        {
            File.WriteAllLines(path, lines);
            Debug.Log($"ĳ���� {characterId}�� isGet�� {newIsGet}�� ���� �Ϸ�� ��.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV ���� ����: {ex.Message}");
        }
    }

    public void ChangeStageStar(int stageNum, int newStar)
    {
        string csvFileName = "StageInfo.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"StageInfo.csv ������ ã�� �� �����ϴ�: {filePath}");
            return;
        }

        // CSV ��ü ������ �о� ����Ʈ�� ��ȯ
        var lines = File.ReadAllLines(filePath).ToList();
        bool updated = false;

        // 0��°�� ���, 1��° ���İ� ������
        for (int i = 1; i < lines.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            var cols = lines[i].Split(',');
            // ù ���� �������� ��ȣ
            if (int.TryParse(cols[0], out int id) && id == stageNum)
            {
                // star�� 14��° �÷� (0���� �����ϹǷ� index 13)
                if (cols.Length > 13)
                {
                    cols[13] = newStar.ToString();
                    lines[i] = string.Join(",", cols);

                    // �޸𸮻��� ����Ʈ�� ����ȭ
                    var stage = stages.Find(s => s.stageNum == stageNum);
                    if (stage != null)
                        stage.star = newStar;

                    Debug.Log($"Stage {stageNum}�� star ���� {newStar}�� ���� �Ϸ�� ��.");
                    updated = true;
                }
                else
                {
                    Debug.LogWarning("CSV �� ������ ���󺸴� ��� star �÷��� ������ �� ���� ��.");
                }
                break;
            }
        }

        if (!updated)
        {
            Debug.LogWarning($"StageInfo.csv���� stageNum {stageNum}�� ã�� �� ���� ��.");
            return;
        }

        // ����� ������ CSV�� �ٽ� ����
        try
        {
            File.WriteAllLines(filePath, lines);
            Debug.Log("StageInfo.csv�� ���������� ������Ʈ�Ǿ��� ��.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV ���� ���� �� ���� �߻�: {ex.Message}");
        }
    }


    public void UpdateCharacterExpAndLevel(int characterId, int newLevel, float newExp)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "TrpgCharacters.csv");
        var lines = File.ReadAllLines(path).ToList();

        for (int i = 1; i < lines.Count; i++)
        {
            var cols = lines[i].Split(',');
            if (!int.TryParse(cols[0], out int id) || id != characterId) continue;

            cols[3] = newLevel.ToString();
            cols[4] = newExp.ToString();
            lines[i] = string.Join(",", cols);
            break;
        }
        File.WriteAllLines(path, lines);
    }
    public void UpdateSkillLevel(int skillId, int newLv)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "SkillInfo.csv");
        var lines = File.ReadAllLines(path).ToList();

        for (int i = 1; i < lines.Count; i++)
        {
            var cols = lines[i].Split(',');
            if (!int.TryParse(cols[0], out int id) || id != skillId) continue;

            cols[3] = newLv.ToString();  // lv �÷� ��ġ
            lines[i] = string.Join(",", cols);
            break;
        }
        File.WriteAllLines(path, lines);
    }
    public void CalculateCurrentStats(CharacterData character)
    {
        // ���� ��� ������
        float levelHp = character.baseHp * (1 + 0.025f * (character.lv - 1));
        float levelAtk = character.baseAtk * (1 + 0.018f * (character.lv - 1));
        float levelMAtk = character.baseMAtk * (1 + 0.018f * (character.lv - 1));
        float levelDef = character.baseDef * (1 + 0.018f * (character.lv - 1));
        float levelMDef = character.baseMDef * (1 + 0.018f * (character.lv - 1));
        float levelCri = character.baseCri * (1 + 0.01f * (character.lv - 1));
        float levelMiss = character.baseMiss * (1 + 0.01f * (character.lv - 1));

        // ���� ���� ����
        character.hp = levelHp;
        character.atk = levelAtk;
        character.mAtk = levelMAtk;
        character.def = levelDef;
        character.mDef = levelMDef;
        character.cri = levelCri;
        character.miss = levelMiss;
    }
}
