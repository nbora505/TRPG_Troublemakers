using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EXSkillManager : MonoBehaviour
{
    [Header("궁극기 일러스트 UI")]
    public GameObject skillCgPanel;          // 최상위 Canvas (Image나 Animator 포함)  
    public Image skillImage;

    [Header("사운드/이펙트 (있으면)")]
    public AudioSource ultimateSfx;             // 궁극기 시전 사운드

    [Header("참조할 매니저/버튼")]
    public BattleManager BM;         // 전투 매니저
    public SkillManager SM;           // 스킬 매니저
    public PlayerController caster;             // 궁극기를 쓸 캐릭터

    [Header("일러스트 애니메이션 길이")]
    public float animationDuration = 1.0f;      // 일러스트 애니메이션이 끝나는 시간 (초, Unscaled)

    [Header("궁극기 이펙트 재생 시간")]
    public float ultimateEffectDuration = 2.0f; // 일러스트 후 이펙트 재생 시간 (Unscaled, 초)

    private void Start()
    {
        
    }

    public void OnExBtnPressed01()
    {
        BM.token[0].GetComponent<Button>().interactable = false;
        caster = BM.playerList.Find
            (p => p.GetComponent<PlayerController>()
            .characterData.id == BM.lineUp[0].id)
            .GetComponent<PlayerController>();

        StartCoroutine(UltimateSequence());
    }
    public void OnExBtnPressed02()
    {
        BM.token[1].GetComponent<Button>().interactable = false;
        caster = BM.playerList.Find
            (p => p.GetComponent<PlayerController>()
            .characterData.id == BM.lineUp[1].id)
            .GetComponent<PlayerController>();

        StartCoroutine(UltimateSequence());
    }
    public void OnExBtnPressed03()
    {
        BM.token[2].GetComponent<Button>().interactable = false;
        caster = BM.playerList.Find
            (p => p.GetComponent<PlayerController>()
            .characterData.id == BM.lineUp[2].id)
            .GetComponent<PlayerController>();

        StartCoroutine(UltimateSequence());
    }
    public void OnExBtnPressed04()
    {
        BM.token[3].GetComponent<Button>().interactable = false;
        caster = BM.playerList.Find
            (p => p.GetComponent<PlayerController>()
            .characterData.id == BM.lineUp[3].id)
            .GetComponent<PlayerController>();

        StartCoroutine(UltimateSequence());
    }
    public void OnExBtnPressed05()
    {
        BM.token[4].GetComponent<Button>().interactable = false;
        caster = BM.playerList.Find
            (p => p.GetComponent<PlayerController>()
            .characterData.id == BM.lineUp[4].id)
            .GetComponent<PlayerController>();

        StartCoroutine(UltimateSequence());
    }

    IEnumerator UltimateSequence()
    {
        // ───────────────── 1) 게임 일시정지 ─────────────────
        Time.timeScale = 0f;

        // ───────────────── 2) 일러스트 (1초) ─────────────────
        skillCgPanel.SetActive(true);
        skillImage.sprite = Resources.Load<Sprite>("EX/" + caster.characterData.id);
        if (ultimateSfx != null) ultimateSfx.Play();

        float t = 0f;
        while (t < animationDuration)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        skillCgPanel.SetActive(false);

        // ───────────────── 준비: exSkillData와 realTarget 미리 구하기 ─────────────────
        SkillData exSkillData = CSVReader.instance.skills
            .FirstOrDefault(s => s.id == caster.characterData.exSkill);

        // realTarget을 이 스코프에서 선언
        PlayerController realTarget = null;
        if (exSkillData != null)
        {
            // decoy 포함 정확한 A타겟 계산
            SM.SetTarget(exSkillData, caster, ref realTarget);
        }

        // ───────────────── 3) 이펙트 (2초) ─────────────────
        if (exSkillData != null)
        {
            if (exSkillData.targetA == "enemyAll")
            {
                foreach (var go in BattleManager.instance.enemyList)
                    SpawnUltimateEffectAt(go.transform.position);
            }
            else if (exSkillData.targetA == "teamAll")
            {
                foreach (var go in BattleManager.instance.playerList)
                    SpawnUltimateEffectAt(go.transform.position);
            }
            else
            {
                // 단일 타겟
                Vector3 pos = realTarget != null
                    ? realTarget.transform.position
                    : caster.transform.position;
                SpawnUltimateEffectAt(pos);
            }

            t = 0f;
            while (t < ultimateEffectDuration)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // ───────────────── 4) 실제 스킬 적용 (일시정지 상태에서!) ─────────────────
        if (exSkillData != null)
        {
            Debug.Log($":::::::::{caster.characterData.name}의 {exSkillData.name} 발동!:::::::::::");

            if (exSkillData.targetA == "enemyAll" || exSkillData.targetA == "teamAll")
            {
                // 더미로 자기 자신(caster) 넘겨 주기
                SM.UseSkill(exSkillData, caster, caster);
            }
            else
            {
                SM.UseSkill(exSkillData, caster, realTarget);
            }
        }

        // ───────────────── 5) 시간 복귀 ─────────────────
        Time.timeScale = BM.timeTwice ? 2f : 1f;
        caster.curMP = 0f;
    }

    private void SpawnUltimateEffectAt(Vector3 position)
    {
        string EX;

        switch (caster.characterData.id)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                EX = $"EX0{caster.characterData.id}";
                break;
            default:
                EX = $"EX{caster.characterData.id}";
                break;
        }

        // 풀 매니저에서 “궁극기 이펙트”를 꺼내온 뒤
        var ultiEffect = PoolManager.Instance.SpawnEffect(EX, position, Quaternion.identity);

        // 만약 자동으로 큐에 돌아가지 않는 풀이라면, 2초 뒤 수동으로 비활성화
        // (그러나 PoolManager가 DespawnAfter를 이용해 자동 반환해 주면 생략해도 됩니다.)
        if (ultiEffect != null)
            StartCoroutine(DisableAfterUnscaled(ultiEffect, ultimateEffectDuration));
    }

    private IEnumerator DisableAfterUnscaled(GameObject go, float delay)
    {
        float t = 0f;
        while (t < delay)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        go.SetActive(false);
    }
}
