using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterData
{
    public int id;
    public string name;
    public int star;

    public int lv;
    public float exp;

    // 기본 베이스 스탯
    public float baseHp;
    public float baseAtk;
    public float baseMAtk;
    public float baseDef;
    public float baseMDef;
    public float baseCri;
    public float baseMiss;

    // 동적 계산된 현재 스탯
    public float hp;
    public float atk;
    public float mAtk;
    public float def;
    public float mDef;
    public float cri;
    public float miss;

    public float range;

    public int[] pattern = new int[6];
    public int[] skill = new int[3];
    public int exSkill;

    public string faceSprite;
    public string standingSprite;
    public string sdSprite;

    public bool isGet;
}