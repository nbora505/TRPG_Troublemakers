using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public void GeneratePotions(int stage,
    out int potionS, out int potionM,
    out int potionL, out int potionXL)
    {
        // 소 포션: 1~4 (stage1), 그 후 [4*(stage-1) ~ 4*stage]
        potionS = Random.Range(
            Mathf.Max(1, 4 * (stage - 1)),
            4 * stage + 1
        );

        // 중 포션: stage<3 → 0
        //         stage==3 → 1~3
        //         stage>3 → [3*(stage-3) ~ 3*(stage-2)]
        potionM = (stage < 3) ? 0
            : Random.Range(
                (stage == 3 ? 1 : 3 * (stage - 3)),
                (stage == 3 ? 3 : 3 * (stage - 2)) + 1
            );

        // 대 포션: stage<6 → 0, 그 외 [stage-5 ~ stage-4]
        potionL = (stage < 6) ? 0
            : Random.Range(
                stage - 5,
                (stage - 4) + 1
            );

        // 특대 포션: stage<9 → 0, 그 외 [stage-8 ~ stage-7]
        potionXL = (stage < 9) ? 0
            : Random.Range(
                stage - 8,
                (stage - 7) + 1
            );
    }
    public int GetCoins(int stage)
    {
        // 스테이지별 기본 코인: stage * 10
        int minCoins = stage * 1000;
        int maxCoins = stage * 1500;     // 최대값(포함하려면 +1 필요)

        // Random.Range의 두 번째 인자는 exclusive(미만)이므로 +1
        return Random.Range(minCoins, maxCoins + 1);
    }
}
