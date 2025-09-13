// ���� ���� �ý��� ���պ�
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

    // ����ġ ���� (������ �ʿ� ����ġ)
    public int GetExpForLevel(int level)
    {
        float baseExp = 50f;
        float growthRate = 1.3f;
        return Mathf.RoundToInt(baseExp * Mathf.Pow(level, growthRate));
    }

    // ��� ��ȭ�� �ʿ��� ���� ��
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

    // ����ġ �߰� �� ������ ó�� (��, ������ �׻� ���� ���)
    public void AddExp(CharacterData character, int addExp)
    {
        character.exp += addExp;

        while (character.exp >= GetExpForLevel(character.lv))
        {
            character.exp -= GetExpForLevel(character.lv);
            character.lv++;
            Debug.Log($"������! {character.name} �� {character.lv}����");
        }

        CSVReader.instance.UpdateCharacterExpAndLevel(character.id, character.lv, character.exp);
        SortManager.instance.MakeCharacterBtns();
        SortManager.instance.SortItems();
    }

    #region Levels
    // ����ġ ���� ��ư��
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
        // ���� ����ġ �߰�
        AddExp(data, expAmount);

        // ������ ���� ����
        potionItem.cnt -= 1;

        // CSV ����
        CSVReader.instance.ChangeItemCount(potionItem.id, -1);

        // UI ����
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

    //��ų ���� ��ȭ
    public void UpgradeSkill(SkillData skill)
    {
        int cost = skill.lv * 500;

        if (CSVReader.instance.playerData.coin < cost)
        {
            Debug.Log("��� ����!");
            return;
        }

        CSVReader.instance.playerData.coin -= cost;
        skill.lv++;

        // CSV �ݿ�
        CSVReader.instance.UpdateSkillLevel(skill.id, skill.lv);
        CSVReader.instance.ChangePlayerStat("Coin", -cost);

        Debug.Log($"��ų {skill.name} ��ȭ �Ϸ�! ���� Lv.{skill.lv}");
    }
    #endregion

    #region Stars
    // ��� �±� ó��
    public void StarUpgradeBtn()
    {
        PromoteCharacter(data, items[data.id - 1]);

        // UI ����
        UIManager.instance.SetCharacterStarPage(data);
        UIManager.instance.SetCharacterSelectPanel(data);
    }
    public void PromoteCharacter(CharacterData character, ItemData pieceItem)
    {
        int requiredPiece = GetRequiredPiece(character.star);

        if (pieceItem.cnt < requiredPiece)
        {
            Debug.Log("������ �����մϴ�.");
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

        Debug.Log($"��� ��ȭ ����! {character.name} �� {character.star}��");
    }
    #endregion

    // �Ź� ���� ���� Ȥ�� UI ���Ž� ȣ�� (���� ���� ���� ���)
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
