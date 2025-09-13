using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantManager : MonoBehaviour
{
    public static EnchantManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public int GetExpForLevel(int level)
    {
        float baseExp = 50f;         // 기본 경험치
        float growthRate = 1.3f;     // 성장 계수

        return Mathf.RoundToInt(baseExp * Mathf.Pow(level, growthRate));
    }
    public int GetAllExp(int level)
    {
        float baseExp = 50f;
        float growthRate = 1.3f;
        int totalExp = 0;

        for (int i = 1; i <= level; i++)
        {
            totalExp += Mathf.RoundToInt(baseExp * Mathf.Pow(i, growthRate));
        }

        return totalExp;
    }
    //경험치 추가 및 레벨업 처리 함수
    public void AddExp(CharacterData character, int addExp)
    {
        character.exp += addExp;

        while (character.exp >= GetExpForLevel(character.lv))
        {
            character.exp -= GetExpForLevel(character.lv);
            character.lv++;
            Debug.Log($"레벨업! {character.name} → {character.lv}레벨");
        }

        // CSV 동기화 (CSVReader에서 구현되어 있다고 가정)
        CSVReader.instance.ChangeCharacterStat(character.id, "lv", character.lv - character.lv); // 그대로 유지 (혹시 안 바뀌면 유지)
        CSVReader.instance.ChangeCharacterStat(character.id, "exp", character.exp - character.exp);
    }

    //경험치 포션 버튼 연결용 함수들
    public void UseExpS(CharacterData character)  // 소형
    {
        if (CSVReader.instance.items[61].cnt < 1) return;
        AddExp(character, 50);
    }

    public void UseExpM(CharacterData character)  // 중형
    {
        if (CSVReader.instance.items[62].cnt < 1) return;
        AddExp(character, 100);
    }

    public void UseExpL(CharacterData character)  // 대형
    {
        if (CSVReader.instance.items[63].cnt < 1) return;
        AddExp(character, 300);
    }

    public void UseExpXL(CharacterData character)  // 특대
    {
        if (CSVReader.instance.items[64].cnt < 1) return;
        AddExp(character, 1000);
    }
}
