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
        float baseExp = 50f;         // �⺻ ����ġ
        float growthRate = 1.3f;     // ���� ���

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
    //����ġ �߰� �� ������ ó�� �Լ�
    public void AddExp(CharacterData character, int addExp)
    {
        character.exp += addExp;

        while (character.exp >= GetExpForLevel(character.lv))
        {
            character.exp -= GetExpForLevel(character.lv);
            character.lv++;
            Debug.Log($"������! {character.name} �� {character.lv}����");
        }

        // CSV ����ȭ (CSVReader���� �����Ǿ� �ִٰ� ����)
        CSVReader.instance.ChangeCharacterStat(character.id, "lv", character.lv - character.lv); // �״�� ���� (Ȥ�� �� �ٲ�� ����)
        CSVReader.instance.ChangeCharacterStat(character.id, "exp", character.exp - character.exp);
    }

    //����ġ ���� ��ư ����� �Լ���
    public void UseExpS(CharacterData character)  // ����
    {
        if (CSVReader.instance.items[61].cnt < 1) return;
        AddExp(character, 50);
    }

    public void UseExpM(CharacterData character)  // ����
    {
        if (CSVReader.instance.items[62].cnt < 1) return;
        AddExp(character, 100);
    }

    public void UseExpL(CharacterData character)  // ����
    {
        if (CSVReader.instance.items[63].cnt < 1) return;
        AddExp(character, 300);
    }

    public void UseExpXL(CharacterData character)  // Ư��
    {
        if (CSVReader.instance.items[64].cnt < 1) return;
        AddExp(character, 1000);
    }
}
