using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PooledText : MonoBehaviour
{
    public TextMesh tmpText;  // �ν����Ϳ��� ����

    // ��ȯ �ø��� ȣ���� �޼���
    public void SetText(string message)
    {
        tmpText.text = message;
    }
}
