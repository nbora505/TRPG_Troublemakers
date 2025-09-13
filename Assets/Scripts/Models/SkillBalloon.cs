using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBalloon : MonoBehaviour
{
    void Start()
    {
        
    }

    IEnumerator BalloonDisappear()
    {
        yield return new WaitForSeconds(1);
    }
}
