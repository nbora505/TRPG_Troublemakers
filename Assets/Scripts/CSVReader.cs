using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using System.Linq;

public class CSVReader : MonoBehaviour
{
    // 캐릭터 정보를 저장할 리스트 (사용자 정의 클래스 사용 가능)
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
            DontDestroyOnLoad(gameObject);  // 첫 생성 때만 파괴 방지
        }
        else if (instance != this)
        {
            Destroy(gameObject);            // 이미 있으면 자기 자신 파괴
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

        // 줄바꿈(\n) 기준으로 분리하여 각 줄을 읽음
        string[] lines = data.Split('\n');

        // 첫 줄이 헤더인 경우 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // 빈 줄 체크

            // 쉼표로 분리
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
                // 빈 문자열 또는 "1"/"0" 같은 경우 처리하고 싶으면 여기에 추가 로직
                newCharacter.isGet = false; // 기본값
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
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + filePath);
            return;
        }

        // CSV 파일의 모든 줄을 읽어옵니다.
        string[] lines = File.ReadAllLines(filePath);

        // 새로운 플레이어 데이터 객체를 생성합니다.
        PlayerData newPlayerData = new PlayerData();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // 각 줄을 콤마(,)로 분리합니다.
            string[] columns = line.Split(',');
            // 첫 번째 항목은 키, 두 번째 항목은 값(존재할 경우)입니다.
            string key = columns[0].Trim();
            string value = columns.Length >= 2 ? columns[1].Trim() : "";

            // 키에 따라 플레이어 데이터의 필드를 설정합니다.
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
                        Debug.LogWarning("LV 값을 파싱할 수 없습니다: " + value);
                    break;
                case "Exp":
                    if (int.TryParse(value, out int exp))
                        newPlayerData.exp = exp;
                    else
                        Debug.LogWarning("Exp 값을 파싱할 수 없습니다: " + value);
                    break;
                case "Energy":
                    if (int.TryParse(value, out int energy))
                        newPlayerData.energy = energy;
                    else
                        Debug.LogWarning("Energy 값을 파싱할 수 없습니다: " + value);
                    break;
                case "Coin":
                    if (int.TryParse(value, out int coin))
                        newPlayerData.coin = coin;
                    else
                        Debug.LogWarning("Coin 값을 파싱할 수 없습니다: " + value);
                    break;
                case "Jewel":
                    if (int.TryParse(value, out int jewel))
                        newPlayerData.jewel = jewel;
                    else
                        Debug.LogWarning("Jewel 값을 파싱할 수 없습니다: " + value);
                    break;
                case "isFirstTime":
                    if (bool.TryParse(value, out bool isFirstTime))
                        newPlayerData.isFirstTime = isFirstTime;
                    else
                        Debug.LogWarning("isFirstTime 값을 파싱할 수 없습니다: " + value);
                    break;
                case "LineUp1":
                    if (int.TryParse(value, out int lineup1))
                        newPlayerData.lineUp[0] = lineup1;
                    else
                        Debug.LogWarning("LineUp1 값을 파싱할 수 없습니다: " + value);
                    break;
                case "LineUp2":
                    if (int.TryParse(value, out int lineup2))
                        newPlayerData.lineUp[1] = lineup2;
                    else
                        Debug.LogWarning("LineUp2 값을 파싱할 수 없습니다: " + value);
                    break;
                case "LineUp3":
                    if (int.TryParse(value, out int lineup3))
                        newPlayerData.lineUp[2] = lineup3;
                    else
                        Debug.LogWarning("LineUp3 값을 파싱할 수 없습니다: " + value);
                    break;
                case "LineUp4":
                    if (int.TryParse(value, out int lineup4))
                        newPlayerData.lineUp[3] = lineup4;
                    else
                        Debug.LogWarning("LineUp4 값을 파싱할 수 없습니다: " + value);
                    break;
                case "LineUp5":
                    if (int.TryParse(value, out int lineup5))
                        newPlayerData.lineUp[4] = lineup5;
                    else
                        Debug.LogWarning("LineUp5 값을 파싱할 수 없습니다: " + value);
                    break;
                default:
                    Debug.LogWarning("알 수 없는 필드명: " + key);
                    break;
            }
            //Debug.LogWarning(value);
        }

        playerData = newPlayerData;
        Debug.Log("플레이어 데이터 로드 성공: " + newPlayerData.name);
    }
    public void ParseEnemyCSV()
    {
        string csvFileName = "EnemyInfo.csv";

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        string data = File.ReadAllText(filePath);

        // 줄바꿈(\n) 기준으로 분리하여 각 줄을 읽음
        string[] lines = data.Split('\n');

        // 첫 줄이 헤더인 경우 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // 빈 줄 체크

            // 쉼표로 분리
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

        // 줄바꿈(\n) 기준으로 분리하여 각 줄을 읽음
        string[] lines = data.Split('\n');

        // 첫 줄이 헤더인 경우 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // 빈 줄 체크

            // 쉼표로 분리
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

        // 줄바꿈(\n) 기준으로 분리하여 각 줄을 읽음
        string[] lines = data.Split('\n');

        // 첫 줄이 헤더인 경우 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // 빈 줄 체크

            // 쉼표로 분리
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

        // 줄바꿈(\n) 기준으로 분리하여 각 줄을 읽음
        string[] lines = data.Split('\n');

        // 첫 줄이 헤더인 경우 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // 빈 줄 체크

            // 쉼표로 분리
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

        // 줄바꿈(\n) 기준으로 분리하여 각 줄을 읽음
        string[] lines = data.Split('\n');

        // 첫 줄이 헤더인 경우 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;  // 빈 줄 체크

            // 쉼표로 분리
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
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + filePath);
            return;
        }

        // CSV 파일의 모든 줄을 리스트 형태로 읽어옵니다.
        List<string> lines = new List<string>(File.ReadAllLines(filePath));
        bool statUpdated = false;

        // 각 줄을 순회하며 "stat" 키를 찾습니다.
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
                        // 새로운 값으로 업데이트된 줄 재구성
                        lines[i] = stat + "," + currentStat.ToString();
                        // 메모리 상의 playerData도 업데이트
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

                        Debug.Log(stat + "업데이트 완료: " + currentStat);
                        statUpdated = true;
                    }
                    else
                    {
                        Debug.LogWarning("값을 파싱할 수 없습니다: " + columns[1]);
                    }
                }
                break;
            }
        }

        if (!statUpdated)
        {
            Debug.LogWarning("키를 CSV 파일에서 찾을 수 없습니다.");
            return;
        }

        // 업데이트된 내용을 CSV 파일에 다시 씁니다.
        try
        {
            File.WriteAllLines(filePath, lines.ToArray());
            Debug.Log("CSV 파일이 성공적으로 업데이트되었습니다.");
        }
        catch (IOException ex)
        {
            Debug.LogError("CSV 파일 쓰기 중 오류 발생: " + ex.Message);
        }
    }

    public void ChangeItemCount(int itemId, int amount)
    {
        string csvFileName = "Items.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        // 파일 전체를 읽어서 라인 단위 리스트로 변환
        List<string> lines = new List<string>(File.ReadAllLines(filePath));
        bool updated = false;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cols = line.Split(',');
            // 첫 열이 아이템 ID
            if (int.TryParse(cols[0], out int id) && id == itemId)
            {
                // 4번째 열(cnt) 파싱
                if (int.TryParse(cols[3], out int cnt))
                {
                    cnt += amount;
                    if (cnt < 0) cnt = 0;  // 음수 방지
                    cols[3] = cnt.ToString();

                    // 업데이트된 라인으로 교체
                    lines[i] = string.Join(",", cols);
                    updated = true;

                    // 메모리상의 items 리스트도 동기화
                    ItemData item = items.Find(x => x.id == itemId);
                    if (item != null)
                    {
                        item.cnt = cnt;
                        Debug.Log($"[CSVReader] 아이템 \"{item.name}\" 개수 업데이트: {cnt}");
                    }
                    else
                    {
                        Debug.LogWarning($"[CSVReader] 메모리상의 items에서 ID {itemId}를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning($"[CSVReader] Items.csv의 cnt 파싱 실패: {cols[3]}");
                }
                break;
            }
        }

        if (!updated)
        {
            Debug.LogWarning($"[CSVReader] Items.csv에서 ID {itemId}를 찾을 수 없습니다.");
            return;
        }

        // 변경된 내용을 파일에 기록
        try
        {
            File.WriteAllLines(filePath, lines.ToArray());
            Debug.Log("Items.csv 파일이 성공적으로 업데이트되었습니다.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"[CSVReader] CSV 파일 쓰기 중 오류 발생: {ex.Message}.");
        }
    }

    // CSV 내에 stat→칼럼 인덱스를 미리 매핑해두고, 한 번에 처리한다.
    static readonly Dictionary<string, int> charFieldMap = new Dictionary<string, int>
{
    {"lv",       3},  {"exp",      4},  {"hp",      5},  {"atk",     6},
    {"mAtk",     7},  {"def",      8},  {"mDef",    9},  {"cri",    10},
    {"miss",    11},  {"range",   12},  {"star",    2},  {"isGet",  26}
};

    public void ChangeCharacterStat(int characterId, string stat, float amount)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "TrpgCharacters.csv");
        if (!File.Exists(path)) { Debug.LogError($"CSV 못 찾겠음: {path}"); return; }

        var lines = File.ReadAllLines(path).ToList();
        for (int i = 1; i < lines.Count; i++)
        {
            var cols = lines[i].Split(',');
            if (!int.TryParse(cols[0], out int id) || id != characterId) continue;

            if (!charFieldMap.TryGetValue(stat, out int idx))
            {
                Debug.LogWarning($"알 수 없는 필드: {stat}"); return;
            }

            // 숫자 필드만 처리 (star, isGet은 float로 바꿀 수 있음)
            float cur = float.Parse(cols[idx]);
            float updated = cur + amount;
            cols[idx] = updated.ToString();

            // 메모리 동기화
            var chr = characters.Find(c => c.id == characterId);
            if (chr != null)
            {
                // reflection 대신 switch로 간단 매핑
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
        Debug.Log($"{characterId}의 {stat}을(를) {amount}만큼 변경 완료.");
    }

    // 지정한 캐릭터의 isGet 플래그(true/false)만 토글하듯 변경
    public void ChangeCharacterIsGet(int characterId, bool newIsGet)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "TrpgCharacters.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV 못 찾겠다 해: {path}");
            return;
        }

        var lines = File.ReadAllLines(path).ToList();
        bool done = false;

        for (int i = 1; i < lines.Count; i++)  // 0번째는 헤더
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var cols = lines[i].Split(',');
            if (!int.TryParse(cols[0], out int id) || id != characterId) continue;

            // CSV 컬럼 26이 isGet 위치
            cols[26] = newIsGet.ToString().ToLower();
            lines[i] = string.Join(",", cols);

            // 메모리 동기화
            var chr = characters.Find(c => c.id == characterId);
            if (chr != null) chr.isGet = newIsGet;

            done = true;
            break;
        }

        if (!done)
        {
            Debug.LogWarning($"ID {characterId} 캐릭터 못 찾겠다 해.");
            return;
        }

        try
        {
            File.WriteAllLines(path, lines);
            Debug.Log($"캐릭터 {characterId}의 isGet을 {newIsGet}로 변경 완료다 해.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 쓰기 실패: {ex.Message}");
        }
    }

    public void ChangeStageStar(int stageNum, int newStar)
    {
        string csvFileName = "StageInfo.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"StageInfo.csv 파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        // CSV 전체 라인을 읽어 리스트로 변환
        var lines = File.ReadAllLines(filePath).ToList();
        bool updated = false;

        // 0번째는 헤더, 1번째 이후가 데이터
        for (int i = 1; i < lines.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            var cols = lines[i].Split(',');
            // 첫 열이 스테이지 번호
            if (int.TryParse(cols[0], out int id) && id == stageNum)
            {
                // star는 14번째 컬럼 (0부터 시작하므로 index 13)
                if (cols.Length > 13)
                {
                    cols[13] = newStar.ToString();
                    lines[i] = string.Join(",", cols);

                    // 메모리상의 리스트도 동기화
                    var stage = stages.Find(s => s.stageNum == stageNum);
                    if (stage != null)
                        stage.star = newStar;

                    Debug.Log($"Stage {stageNum}의 star 값을 {newStar}로 변경 완료다 해.");
                    updated = true;
                }
                else
                {
                    Debug.LogWarning("CSV 열 개수가 예상보다 적어서 star 컬럼에 접근할 수 없다 해.");
                }
                break;
            }
        }

        if (!updated)
        {
            Debug.LogWarning($"StageInfo.csv에서 stageNum {stageNum}를 찾을 수 없다 해.");
            return;
        }

        // 변경된 내용을 CSV에 다시 쓰기
        try
        {
            File.WriteAllLines(filePath, lines);
            Debug.Log("StageInfo.csv가 성공적으로 업데이트되었다 해.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일 쓰기 중 오류 발생: {ex.Message}");
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

            cols[3] = newLv.ToString();  // lv 컬럼 위치
            lines[i] = string.Join(",", cols);
            break;
        }
        File.WriteAllLines(path, lines);
    }
    public void CalculateCurrentStats(CharacterData character)
    {
        // 레벨 기반 곱연산
        float levelHp = character.baseHp * (1 + 0.025f * (character.lv - 1));
        float levelAtk = character.baseAtk * (1 + 0.018f * (character.lv - 1));
        float levelMAtk = character.baseMAtk * (1 + 0.018f * (character.lv - 1));
        float levelDef = character.baseDef * (1 + 0.018f * (character.lv - 1));
        float levelMDef = character.baseMDef * (1 + 0.018f * (character.lv - 1));
        float levelCri = character.baseCri * (1 + 0.01f * (character.lv - 1));
        float levelMiss = character.baseMiss * (1 + 0.01f * (character.lv - 1));

        // 최종 스탯 적용
        character.hp = levelHp;
        character.atk = levelAtk;
        character.mAtk = levelMAtk;
        character.def = levelDef;
        character.mDef = levelMDef;
        character.cri = levelCri;
        character.miss = levelMiss;
    }
}
