using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;
    public string id;
    public string pw;

    public int lv;
    public int exp;

    public int energy;
    public int coin;
    public int jewel;

    public int[] lineUp = new int[5];

    public bool isFirstTime;
}
