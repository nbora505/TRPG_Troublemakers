using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    public AreaPanel areaPanel;
    public StageData stageData;

    public void GetSelectedStage()
    {
        areaPanel.selecetedStage = stageData;
    }
}
