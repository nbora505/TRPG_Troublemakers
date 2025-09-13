using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EXSkillManager : MonoBehaviour
{
    [Header("�ñر� �Ϸ���Ʈ UI")]
    public GameObject skillCgPanel;          // �ֻ��� Canvas (Image�� Animator ����)  
    public Image skillImage;

    [Header("����/����Ʈ (������)")]
    public AudioSource ultimateSfx;             // �ñر� ���� ����

    [Header("������ �Ŵ���/��ư")]
    public BattleManager BM;         // ���� �Ŵ���
    public SkillManager SM;           // ��ų �Ŵ���
    public PlayerController caster;             // �ñر⸦ �� ĳ����

    [Header("�Ϸ���Ʈ �ִϸ��̼� ����")]
    public float animationDuration = 1.0f;      // �Ϸ���Ʈ �ִϸ��̼��� ������ �ð� (��, Unscaled)

    [Header("�ñر� ����Ʈ ��� �ð�")]
    public float ultimateEffectDuration = 2.0f; // �Ϸ���Ʈ �� ����Ʈ ��� �ð� (Unscaled, ��)

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
        // ���������������������������������� 1) ���� �Ͻ����� ����������������������������������
        Time.timeScale = 0f;

        // ���������������������������������� 2) �Ϸ���Ʈ (1��) ����������������������������������
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

        // ���������������������������������� �غ�: exSkillData�� realTarget �̸� ���ϱ� ����������������������������������
        SkillData exSkillData = CSVReader.instance.skills
            .FirstOrDefault(s => s.id == caster.characterData.exSkill);

        // realTarget�� �� ���������� ����
        PlayerController realTarget = null;
        if (exSkillData != null)
        {
            // decoy ���� ��Ȯ�� AŸ�� ���
            SM.SetTarget(exSkillData, caster, ref realTarget);
        }

        // ���������������������������������� 3) ����Ʈ (2��) ����������������������������������
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
                // ���� Ÿ��
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

        // ���������������������������������� 4) ���� ��ų ���� (�Ͻ����� ���¿���!) ����������������������������������
        if (exSkillData != null)
        {
            Debug.Log($":::::::::{caster.characterData.name}�� {exSkillData.name} �ߵ�!:::::::::::");

            if (exSkillData.targetA == "enemyAll" || exSkillData.targetA == "teamAll")
            {
                // ���̷� �ڱ� �ڽ�(caster) �Ѱ� �ֱ�
                SM.UseSkill(exSkillData, caster, caster);
            }
            else
            {
                SM.UseSkill(exSkillData, caster, realTarget);
            }
        }

        // ���������������������������������� 5) �ð� ���� ����������������������������������
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

        // Ǯ �Ŵ������� ���ñر� ����Ʈ���� ������ ��
        var ultiEffect = PoolManager.Instance.SpawnEffect(EX, position, Quaternion.identity);

        // ���� �ڵ����� ť�� ���ư��� �ʴ� Ǯ�̶��, 2�� �� �������� ��Ȱ��ȭ
        // (�׷��� PoolManager�� DespawnAfter�� �̿��� �ڵ� ��ȯ�� �ָ� �����ص� �˴ϴ�.)
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
