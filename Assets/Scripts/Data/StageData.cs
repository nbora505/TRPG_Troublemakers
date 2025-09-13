using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public int stageNum;
    public string stageName;
    public int[] enemyId = new int[5];
    public float[] enemyPos = new float[5];
    public string stageBG;
    public int star;
}
