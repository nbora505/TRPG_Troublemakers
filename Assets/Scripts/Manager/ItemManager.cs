using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public void GeneratePotions(int stage,
    out int potionS, out int potionM,
    out int potionL, out int potionXL)
    {
        // �� ����: 1~4 (stage1), �� �� [4*(stage-1) ~ 4*stage]
        potionS = Random.Range(
            Mathf.Max(1, 4 * (stage - 1)),
            4 * stage + 1
        );

        // �� ����: stage<3 �� 0
        //         stage==3 �� 1~3
        //         stage>3 �� [3*(stage-3) ~ 3*(stage-2)]
        potionM = (stage < 3) ? 0
            : Random.Range(
                (stage == 3 ? 1 : 3 * (stage - 3)),
                (stage == 3 ? 3 : 3 * (stage - 2)) + 1
            );

        // �� ����: stage<6 �� 0, �� �� [stage-5 ~ stage-4]
        potionL = (stage < 6) ? 0
            : Random.Range(
                stage - 5,
                (stage - 4) + 1
            );

        // Ư�� ����: stage<9 �� 0, �� �� [stage-8 ~ stage-7]
        potionXL = (stage < 9) ? 0
            : Random.Range(
                stage - 8,
                (stage - 7) + 1
            );
    }
    public int GetCoins(int stage)
    {
        // ���������� �⺻ ����: stage * 10
        int minCoins = stage * 1000;
        int maxCoins = stage * 1500;     // �ִ밪(�����Ϸ��� +1 �ʿ�)

        // Random.Range�� �� ��° ���ڴ� exclusive(�̸�)�̹Ƿ� +1
        return Random.Range(minCoins, maxCoins + 1);
    }
}
