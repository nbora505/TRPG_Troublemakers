using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PooledText : MonoBehaviour
{
    public TextMesh tmpText;  // 인스펙터에서 연결

    // 소환 시마다 호출할 메서드
    public void SetText(string message)
    {
        tmpText.text = message;
    }
}
