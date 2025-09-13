// 동적 성장 시스템 통합본
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class GrowthManager : MonoBehaviour
{
    public static GrowthManager instance;
    public CharacterData data;
    public List<SkillData> skills;
    public List<ItemData> items;

    private void Awake()
    {
        instance = this;
        skills = CSVReader.instance.skills;
        items = CSVReader.instance.items;
    }

    // 경험치 공식 (레벨업 필요 경험치)
    public int GetExpForLevel(int level)
    {
        float baseExp = 50f;
        float growthRate = 1.3f;
        return Mathf.RoundToInt(baseExp * Mathf.Pow(level, growthRate));
    }

    // 등급 강화에 필요한 조각 수
    public int GetRequiredPiece(int star)
    {
        switch (star)
        {
            case 1: return 30;
            case 2: return 100;
            case 3: return 120;
            case 4: return 150;
            default: return 9999;
        }
    }

    // 경험치 추가 및 레벨업 처리 (단, 스탯은 항상 동적 계산)
    public void AddExp(CharacterData character, int addExp)
    {
        character.exp += addExp;

        while (character.exp >= GetExpForLevel(character.lv))
        {
            character.exp -= GetExpForLevel(character.lv);
            character.lv++;
            Debug.Log($"레벨업! {character.name} → {character.lv}레벨");
        }

        CSVReader.instance.UpdateCharacterExpAndLevel(character.id, character.lv, character.exp);
        SortManager.instance.MakeCharacterBtns();
        SortManager.instance.SortItems();
    }

    #region Levels
    // 경험치 포션 버튼용
    public void UseSmallPotion()
    {
        if (CSVReader.instance.items[61].cnt < 1) return;
        UsePotion(items[61], 50);
    }

    public void UseMediumPotion()
    {
        if (CSVReader.instance.items[62].cnt < 1) return;
        UsePotion(items[62], 100);
    }

    public void UseLargePotion()
    {
        if (CSVReader.instance.items[63].cnt < 1) return;
        UsePotion(items[63], 300);
    }

    public void UseXLargePotion()
    {
        if (CSVReader.instance.items[64].cnt < 1) return;
        UsePotion(items[64], 1000);
    }

    public void UsePotion(ItemData potionItem, int expAmount)
    {
        // 먼저 경험치 추가
        AddExp(data, expAmount);

        // 아이템 개수 감소
        potionItem.cnt -= 1;

        // CSV 갱신
        CSVReader.instance.ChangeItemCount(potionItem.id, -1);

        // UI 갱신
        UIManager.instance.SetCharacterSelectPanel(data);
    }
    #endregion

    #region Skills
    public void OnExSkillBtn()
    {
        SkillData skill = skills.Find(x => x.id == data.exSkill);
        UpgradeSkill(skill);

        UIManager.instance.SetCharacterSkillPage(data);
        StartCoroutine(UIManager.instance.SetPlayerDataToUI());
    }
    public void OnSkill01Btn()
    {
        SkillData skill = skills.Find(x => x.id == data.skill[0]);
        UpgradeSkill(skill);

        UIManager.instance.SetCharacterSkillPage(data);
        StartCoroutine(UIManager.instance.SetPlayerDataToUI());
    }
    public void OnSkill02Btn()
    {
        SkillData skill = skills.Find(x => x.id == data.skill[1]);
        UpgradeSkill(skill);

        UIManager.instance.SetCharacterSkillPage(data);
        StartCoroutine(UIManager.instance.SetPlayerDataToUI());
    }
    public void OnSkill03Btn()
    {
        SkillData skill = skills.Find(x => x.id == data.skill[2]);
        UpgradeSkill(skill);

        UIManager.instance.SetCharacterSkillPage(data);
        StartCoroutine(UIManager.instance.SetPlayerDataToUI());
    }

    //스킬 레벨 강화
    public void UpgradeSkill(SkillData skill)
    {
        int cost = skill.lv * 500;

        if (CSVReader.instance.playerData.coin < cost)
        {
            Debug.Log("골드 부족!");
            return;
        }

        CSVReader.instance.playerData.coin -= cost;
        skill.lv++;

        // CSV 반영
        CSVReader.instance.UpdateSkillLevel(skill.id, skill.lv);
        CSVReader.instance.ChangePlayerStat("Coin", -cost);

        Debug.Log($"스킬 {skill.name} 강화 완료! 현재 Lv.{skill.lv}");
    }
    #endregion

    #region Stars
    // 등급 승급 처리
    public void StarUpgradeBtn()
    {
        PromoteCharacter(data, items[data.id - 1]);

        // UI 갱신
        UIManager.instance.SetCharacterStarPage(data);
        UIManager.instance.SetCharacterSelectPanel(data);
    }
    public void PromoteCharacter(CharacterData character, ItemData pieceItem)
    {
        int requiredPiece = GetRequiredPiece(character.star);

        if (pieceItem.cnt < requiredPiece)
        {
            Debug.Log("조각이 부족합니다.");
            return;
        }

        pieceItem.cnt -= requiredPiece;
        CSVReader.instance.ChangeItemCount(pieceItem.id, -requiredPiece);

        character.star++;
        CSVReader.instance.ChangeCharacterStat(character.id, "star", 1);

        character.baseHp += 200;
        if (character.baseAtk > 0) character.baseAtk += 20;
        if (character.baseMAtk > 0) character.baseMAtk += 20;
        character.baseDef += 1;
        character.baseMDef += 1;
        character.baseCri += 1;
        character.baseMiss += 1;

        Debug.Log($"등급 강화 성공! {character.name} → {character.star}성");
    }
    #endregion

    // 매번 전투 진입 혹은 UI 갱신시 호출 (현재 스탯 동적 계산)
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
