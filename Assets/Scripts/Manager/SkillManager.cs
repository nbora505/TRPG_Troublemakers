using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class SkillManager : MonoBehaviour
{
    bool isEnemyAll;
    bool isTeamAll;

    public void UseSkill(SkillData skill, PlayerController character, PlayerController target)
    {
        //1) Null ���� ���
        if (skill == null)
        {
            Debug.LogWarning("UseSkill ȣ�� �� skill�� null.");
            return;
        }
        if (character == null)
        {
            Debug.LogWarning("UseSkill ȣ�� �� character�� null.");
            return;
        }
        if (target == null)
        {
            Debug.LogWarning($"UseSkill({skill.name}) ȣ�� �� target�� null. character={character.name}");
            return;
        }

        //2) ��ų ��� �÷��� �ʱ�ȭ
        isEnemyAll = false;
        isTeamAll = false;

        //3) ��ų ��� ����
        SetTarget(skill, character, ref target);

        // 3-1) SetTarget���� ���� �� �� ���, �⺻��(���� ù ��) ����
        if (target == null)
        {
            var enemies = BattleManager.instance.enemyList;
            if (enemies.Count > 0)
                target = enemies[0].GetComponent<PlayerController>();
            else
            {
                Debug.LogWarning("���� ��� ��ų ����� ã�� �� �����ϴ�.");
                return;
            }
        }

        // 4) ���� ó���� ��� ����Ʈ ����
        var targetsA = new List<PlayerController>();
        if (isEnemyAll)
        {
            targetsA.AddRange(BattleManager.instance.enemyList
                              .Select(go => go.GetComponent<PlayerController>()));
        }
        else if (isTeamAll)
        {
            targetsA.AddRange(BattleManager.instance.playerList
                              .Select(go => go.GetComponent<PlayerController>()));
        }
        else
        {
            targetsA.Add(target);
        }

        // 5) ��ų ����: try-finally�� �÷��� ������ �ʱ�ȭ
        try
        {
            // ������������������ A ȿ�� ���� ������������������
            foreach (var t in targetsA)
            {
                ApplySingleEffect(skill.typeA, skill, character, t , false);
                SetEffect(skill, character, t);
            }

            // ������������������ B ȿ�� ���� ������������������
            if (!string.IsNullOrEmpty(skill.typeB) && skill.typeB != "0")
            {
                // �÷��� �ʱ�ȭ
                isEnemyAll = false;
                isTeamAll = false;
                // ��� ������
                PlayerController targetB = target;
                SetTargetB(skill, character, ref targetB);

                // ��� ����Ʈ ����
                var targetsB = new List<PlayerController>();
                if (isEnemyAll)
                    targetsB.AddRange(BattleManager.instance.enemyList.Select(go => go.GetComponent<PlayerController>()));
                else if (isTeamAll)
                    targetsB.AddRange(BattleManager.instance.playerList.Select(go => go.GetComponent<PlayerController>()));
                else
                    targetsB.Add(targetB);

                foreach (var t in targetsB)
                {
                    ApplySingleEffect(skill.typeB, skill, character, t, true);
                    SetEffect(skill, character, t);
                }
            }
        }
        finally
        {
            // �÷��� ���� ���� ������ ����
            isEnemyAll = false;
            isTeamAll = false;
        }


        //�� ������ ��ǳ��
        PoolManager.Instance.SpawnEffectWithText
            ("Balloon", character.transform.position, Quaternion.identity, skill.name);
    }

    //��ų �غ��
    #region Set Skill

    void SetTargetB(SkillData skill, PlayerController caster, ref PlayerController target)
    {
        // (1) decoy �켱 ó���� �̹� UseSkill �������� �������� ����
        List<GameObject> el = BattleManager.instance.enemyList;
        List<GameObject> pl = BattleManager.instance.playerList;

        switch (skill.targetB)
        {
            case "enemy":
                if (el.Count > 0)
                    target = el[0].GetComponent<PlayerController>();
                break;

            case "enemyAll":
                isEnemyAll = true;
                break;

            case "enemy5":
                target = el[el.Count - 1].GetComponent<PlayerController>();
                break;

            case "enemy4":
                if (el.Count > 2)
                    target = el[el.Count - 2].GetComponent<PlayerController>();
                else
                    target = el[el.Count - 1].GetComponent<PlayerController>();
                break;

            case "me":
                target = caster;
                break;

            case "teamAll":
                isTeamAll = true;
                break;

            case "team1":
                target = pl[0].GetComponent<PlayerController>();
                break;

            case "team5":
                target = pl[pl.Count - 1].GetComponent<PlayerController>();
                break;

            case "teamHp":
                {
                    var minHpGO = pl
                        .OrderBy(x => x.GetComponent<PlayerController>().curHP)
                        .FirstOrDefault();
                    if (minHpGO != null)
                        target = minHpGO.GetComponent<PlayerController>();
                }
                break;

            case "teamMp":
                {
                    var minMpGO = pl
                        .OrderBy(x => x.GetComponent<PlayerController>().curMP)
                        .FirstOrDefault();
                    if (minMpGO != null)
                        target = minMpGO.GetComponent<PlayerController>();
                }
                break;

            default:
                Debug.LogWarning($"�� �� ���� targetB �� '{skill.targetB}'.");
                break;
        }
    }

    float ConvertSizeToFloat(string size)
    {
        switch(size)
        {
            case "s":
                return 0.6f;
            case "m":
                return 1.2f;
            case "l":
                return 3.6f;
            case "xl":
                return 4.8f;
            default:
                Debug.LogError("��ų ������� �߸� �Էµ�");
                return 0;
        }
    }
    float SetLevelDamage(string size)
    {
        switch (size)
        {
            case "s":
                return 8f;
            case "m":
                return 20f;
            case "l":
                return 45f;
            case "xl":
                return 60f;
            default:
                Debug.LogError("��ų ������� �߸� �Էµ�");
                return 0;
        }
    }
    public void SetTarget(SkillData skill, PlayerController character, ref PlayerController target)
    {
        // ������������������������������������������������������������������������
        // 1) ������ ȿ�� �켱 ó��
        //    (caster.isEnemy�� ���� �� �Ǵ� �Ʊ� ����Ʈ �߿���)
        var list = character.isEnemy
            ? BattleManager.instance.playerList   // ���̸� �÷��̾��Ʈ
            : BattleManager.instance.enemyList;   // �Ʊ��̸� �� ����Ʈ

        var decoyPc = list
            .Select(go => go.GetComponent<PlayerController>())
            .FirstOrDefault(pc => pc.HasEffect(StatusEffect.Type.Decoy));

        if (decoyPc != null)
        {
            target = decoyPc;
            return;   // ������ ������� ��� ����!
        }
        // ������������������������������������������������������������������������

        List<GameObject> el = BattleManager.instance.enemyList;
        List<GameObject> pl = BattleManager.instance.playerList;
        switch (skill.targetA)
        {
            case "enemy"://������ �� �ϳ�
                if (el.Count > 0)
                    target = el[0].GetComponent<PlayerController>();
                break;
            case "enemyAll"://�� ��ü
                isEnemyAll = true;
                break;
            case "enemy5"://���Ĺ��� �� �ϳ�
                target = el[el.Count - 1].GetComponent<PlayerController>();
                break;
            case "enemy4"://�Ĺ��� �� �ϳ�
                if(el.Count > 2) target = el[el.Count - 2].GetComponent<PlayerController>();
                else target = el[el.Count - 1].GetComponent<PlayerController>();
                break;
            case "me"://�ڽ�
                target = character;
                break;
            case "teamAll"://�Ʊ� ��ü
                isTeamAll = true;
                break;
            case "team1"://�������� �Ʊ�
                target = pl[0].GetComponent<PlayerController>();
                break;
            case "team5"://���Ĺ��� �Ʊ�
                target = pl[pl.Count - 1].GetComponent<PlayerController>();
                break;
            case "teamHp"://ü���� ���� ���� �Ʊ�
                var weakest = pl
                    .Select(go => go.GetComponent<PlayerController>())
                    .Where(pc => pc != null)
                    .OrderBy(pc => pc.curHP / pc.characterData.hp)
                    .FirstOrDefault();
                if (weakest != null)
                    target = weakest;
                break;
            case "teamMp"://������ ���� ���� �Ʊ�
                GameObject minMp = pl.OrderBy(x => x.GetComponent<PlayerController>().curMP).FirstOrDefault();
                target = minMp.GetComponent<PlayerController>();
                break;
            default:
                Debug.LogWarning($"�� �� ���� targetA �� '{skill.targetA}'.");
                break;
        }
    }
    bool CheckIsMissed(PlayerController target)
    {
        if(target.curMiss <=0) return false;
        
        float miss = 100 / (1 + 100 / target.curMiss);
        int rand = Random.Range(0, 100);

        if (rand > miss) return false;
        else 
        {
            Debug.Log("MISS!!!!");
            return true;
        }
    }
    void SetEffect(SkillData skill, PlayerController character, PlayerController target)
    {
        // ���� ���ű��� Ȯ����
        string cast = skill.castEffect?.Trim() ?? "";
        string hit = skill.hitEffect?.Trim() ?? "";

        // (1) ����׷� ���ڿ� ���� Ȯ���غ���
        Debug.Log($"[SetEffect] cast:'{cast}', hit:'{hit}'");

        //�ֹ���
        if (cast != "0")
            PoolManager.Instance.SpawnEffect(cast, character.transform.position, Quaternion.identity);

        //���ֹ���
        if (hit != "0")
            PoolManager.Instance.SpawnEffect(hit, target.transform.position, Quaternion.identity);
    }
    #endregion

    //��ų ����
    #region Use Skill

    void ApplySingleEffect(string typeKey, SkillData skill, PlayerController caster, PlayerController target, bool isB)
    {
        // ��duration���� isB �� ���� ��� ���
        float duration = isB ? skill.timeB : skill.timeA;

        switch (typeKey)
        {
            // ����/���� ����
            case "atk":
                UseAtk(skill, caster, target);
                break;
            case "mAtk":
                UseMAtk(skill, caster, target);
                break;

            // ����
            case "buffAtk":
                UseBuff(skill, caster, target, target.characterData.atk, StatusEffect.Type.BuffAtk, duration);
                break;
            case "buffMAtk":
                UseBuff(skill, caster, target, target.characterData.mAtk, StatusEffect.Type.BuffMAtk, duration);
                break;
            case "buffDef":
                UseBuff(skill, caster, target, target.characterData.def, StatusEffect.Type.BuffDef, duration);
                break;
            case "buffMDef":
                UseBuff(skill, caster, target, target.characterData.mDef, StatusEffect.Type.BuffMDef, duration);
                break;
            case "buffCri":
                UseBuff(skill, caster, target, target.characterData.cri, StatusEffect.Type.BuffCri, duration);
                break;
            case "buffMiss":
                UseBuff(skill, caster, target, target.characterData.miss, StatusEffect.Type.BuffMiss, duration);
                break;

            // �����
            case "debuffAtk":
                UseBuff(skill, caster, target, target.characterData.atk, StatusEffect.Type.DebuffAtk, duration);
                break;
            case "debuffMAtk":
                UseBuff(skill, caster, target, target.characterData.mAtk, StatusEffect.Type.DebuffMAtk, duration);
                break;
            case "debuffDef":
                UseBuff(skill, caster, target, target.characterData.def, StatusEffect.Type.DebuffDef, duration);
                break;
            case "debuffMDef":
                UseBuff(skill, caster, target, target.characterData.mDef, StatusEffect.Type.DebuffMDef, duration);
                break;
            case "debuffCri":
                UseBuff(skill, caster, target, target.characterData.cri, StatusEffect.Type.DebuffCri, duration);
                break;
            case "debuffMiss":
                UseBuff(skill, caster, target, target.characterData.miss, StatusEffect.Type.DebuffMiss, duration);
                break;

            // ȸ��
            case "healHp":
                UseHealHP(skill, caster, target);
                break;
            case "healMp":
                UseHealMP(skill, caster, target);
                break;

            // ���� �̻�
            case "stun":
                UseStun(skill, target, duration);
                break;
            case "fire":
                UseBurn(skill, target, duration);
                break;
            case "decoy":
                UseDecoy(skill, caster, duration);
                break;
            case "unDebuff":
                UseCleanse(target);
                break;
            case "joke":
                UseJoke(skill, caster, target);
                break;

            default:
                Debug.LogWarning($"Unknown skill type: {typeKey}");
                break;
        }
    }

    void UseAtk(SkillData skill, PlayerController character, PlayerController target)
    {
        //ȸ�� ����
        if (CheckIsMissed(target))
        {
            PoolManager.Instance.SpawnEffectWithText
                ("Miss", target.transform.position, Quaternion.identity, "Miss!");
            return;
        }

        //������ ���
        float damageSize = ConvertSizeToFloat(skill.sizeA);
        float levelDamage = SetLevelDamage(skill.sizeA);
        float damage = damageSize * character.curAtk + character.characterData.lv * levelDamage;
        float finalDamage = damage / (1 + target.curDef / 100);

        // ũ��Ƽ�� ����
        bool isCrit = Random.value < character.curCri / 100f;
        if (isCrit)
        {
            finalDamage *= 1.5f; // ũ�� ����, ��: 1.5��
            Vector3 newPos = target.transform.position;
            newPos.y += 1f;
            PoolManager.Instance.SpawnEffectWithText
                ("DamageA", newPos, Quaternion.identity, "Critical!");
        }

        target.curHP -= finalDamage;

        //��ų,������ ����Ʈ
        PoolManager.Instance.SpawnEffectWithText
            ("DamageA", target.transform.position, Quaternion.identity, Mathf.Round(finalDamage).ToString());

        Debug.Log(character.name + "(��)�� " + target.name + "���� " + finalDamage + "������!");

    }

    void UseMAtk(SkillData skill, PlayerController character, PlayerController target)
    {
        //ȸ�� ����
        if (CheckIsMissed(target))
        {
            PoolManager.Instance.SpawnEffectWithText
                ("Miss", target.transform.position, Quaternion.identity, "Miss!");
            return;
        }

        //������ ���
        float damageSize = ConvertSizeToFloat(skill.sizeA);
        float levelDamage = SetLevelDamage(skill.sizeA);
        float damage = damageSize * character.curMAtk + character.characterData.lv * levelDamage;
        float finalDamage = damage / (1 + target.curMDef / 100);

        // ũ��Ƽ�� ����
        bool isCrit = Random.value < character.curCri / 100f;
        if (isCrit)
        {
            finalDamage *= 1.5f; // ũ�� ����, ��: 1.5��
            Vector3 newPos = target.transform.position;
            newPos.y += 0.5f;
            PoolManager.Instance.SpawnEffectWithText
                ("DamageM", newPos, Quaternion.identity, "Critical!");
        }

        target.curHP -= finalDamage;

        //��ų,������ ����Ʈ
        PoolManager.Instance.SpawnEffectWithText
            ("DamageM", target.transform.position, Quaternion.identity, Mathf.Round(finalDamage).ToString());

        Debug.Log(character.name + "(��)�� " + target.name + "���� " + finalDamage + "������!");
    }

    void UseBuff(SkillData skill, PlayerController caster, PlayerController t, 
        float buffStat, StatusEffect.Type effectType, float duration)
    {
        // 1) �ۼ�Ʈ ����Ʈ ���
        float levelPoint = SetLevelDamage(skill.sizeA);        // s=8, m=20, ��
        float levelPointBonus = caster.characterData.lv;            // ������ŭ
        float buffPercent = (levelPoint + levelPointBonus) / 100f;

        // 2) ���� ������
        float buffAmount = buffStat * buffPercent;

        // 3) ����
        t.ApplyEffect(effectType, buffAmount, duration);

        Debug.Log($"{caster.characterData.name} �� {t.characterData.name}���� " +
                  $"{effectType}�� {skill.sizeA}��ŭ ����: +{buffPercent * 100f:F1}% (+{buffAmount:F1}), " +
                  $"���ӽð� {duration}s");
    }


    void UseHealHP(SkillData skill, PlayerController caster, PlayerController target)
    {
        if (target == null) return;  // ������ġ

        // 1) ���� ���
        float healSize = ConvertSizeToFloat(skill.sizeA);   // ��: s,m,l, xl�� �����ϴ� ���
        float levelHeal = SetLevelDamage(skill.sizeA);       // ��ų ������ ����ġ

        float healAmount = target.characterData.hp * healSize / 10 + caster.characterData.lv * levelHeal / 100;

        // 2) ����� ü�¿� ���� & �ִ�ġ ĸ
        target.curHP += healAmount;
        if (target.curHP > target.characterData.hp)
            target.curHP = target.characterData.hp;

        // 3) ����Ʈ �� �α�
        PoolManager.Instance.SpawnEffectWithText(
            "Heal", target.transform.position, Quaternion.identity,
            "+" + Mathf.RoundToInt(healAmount).ToString()
        );
        Debug.Log($"{caster.characterData.name}��(��) {target.characterData.name}���� {Mathf.RoundToInt(healAmount)}��ŭ ġ��!");
    }
    void UseHealMP(SkillData skill, PlayerController caster, PlayerController target)
    {
        if (target == null) return;  // ������ġ

        // 1) ���� ���
        float healSize = ConvertSizeToFloat(skill.sizeA);   // ��: s,m,l, xl�� �����ϴ� ���
        float levelHeal = SetLevelDamage(skill.sizeA);       // ��ų ������ ����ġ

        float healAmount = healSize * 10 + caster.characterData.lv * levelHeal / 100;

        // 2) ����� ü�¿� ���� & �ִ�ġ ĸ
        target.curMP += healAmount;
        if (target.curMP > 100)
            target.curMP = 100;

        // 3) ����Ʈ �� �α�
        PoolManager.Instance.SpawnEffectWithText(
            "Heal", target.transform.position, Quaternion.identity,
            "+" + Mathf.RoundToInt(healAmount).ToString()
        );
        Debug.Log($"{caster.characterData.name}��(��) {target.characterData.name}���� {Mathf.RoundToInt(healAmount)}��ŭ ���� ȸ��!");
    }

    void UseStun(SkillData skill, PlayerController target, float duration)
    {
        target.ApplyEffect(StatusEffect.Type.Stun, 0f, duration);
        Debug.Log($"{target.name}�� {duration}�� ���� ����!");
    }
    void UseBurn(SkillData skill, PlayerController target, float duration)
    {
        if (target == null) return;
        // �ڷ�ƾ ����
        target.ApplyEffect(StatusEffect.Type.Burn, 0, duration);
        StartCoroutine(BurnCoroutine(target, duration));
    }
    //0.5�ʸ��� ���� ü���� 3% ������
    private IEnumerator BurnCoroutine(PlayerController target, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && target.curHP > 0f)
        {
            if (target == null) yield break;
            yield return new WaitForSeconds(0.5f);
            // ���� ü���� 3% ���
            float damage = target.curHP * 0.03f;
            target.curHP -= damage;

            // ����Ʈ + �α�
            PoolManager.Instance.SpawnEffectWithText(
                "DamageM",
                target.transform.position,
                Quaternion.identity,
                $"{Mathf.RoundToInt(damage)}"
            );
            elapsed += 0.5f;
        }
    }
    void UseDecoy(SkillData skill, PlayerController caster, float duration)
    {
        caster.ApplyEffect(StatusEffect.Type.Decoy, 0f, duration);
        Debug.Log($"{caster.characterData.name}���� {duration}�� ���� ������ ���� �ο�!");  // Ȯ�ο�
    }
    void UseCleanse(PlayerController target)
    {
        // ������� ���� ����
        var sem = target.GetComponent<StatusEffectManager>();
        sem.RemoveAll(eff => eff.type.ToString().StartsWith("Debuff"));
    }

    // 20% Ȯ���� target�� ���(=���� HP��ŭ ������), ���� �� caster�� ���.
    void UseJoke(SkillData skill, PlayerController caster, PlayerController target)
    {
        if (target == null || caster == null) return;

        // ���� Ȯ�� 20%
        bool success = Random.value < 0.2f;

        if (success)
        {
            // target ���
            float damage = target.curHP;
            target.curHP = 0f;
            PoolManager.Instance.SpawnEffectWithText(
                "DamageA",
                target.transform.position,
                Quaternion.identity,
                $"{Mathf.RoundToInt(damage)}!"
            );
            Debug.Log($"{caster.characterData.name}�� joke ��ų ����! {target.characterData.name} ���!");
        }
        else
        {
            // caster ���
            float selfDamage = caster.curHP;
            caster.curHP = 0f;
            PoolManager.Instance.SpawnEffectWithText(
                "DamageA",
                caster.transform.position,
                Quaternion.identity,
                $"{Mathf.RoundToInt(selfDamage)}!"
            );
            Debug.Log($"{caster.characterData.name}�� joke ��ų ����... �ڽ��� ���.");
        }
    }
    #endregion
}
