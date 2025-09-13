using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public int id;
    public string name;
    public string description;
    public int lv;
    
    public string typeA;
    public string targetA;
    public string sizeA;
    public float timeA;

    public string typeB;
    public string targetB;
    public string sizeB;
    public float timeB;

    public string castEffect;
    public string hitEffect;
}
