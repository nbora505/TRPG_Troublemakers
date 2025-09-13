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
        //1) Null 참조 방어
        if (skill == null)
        {
            Debug.LogWarning("UseSkill 호출 시 skill이 null.");
            return;
        }
        if (character == null)
        {
            Debug.LogWarning("UseSkill 호출 시 character가 null.");
            return;
        }
        if (target == null)
        {
            Debug.LogWarning($"UseSkill({skill.name}) 호출 시 target이 null. character={character.name}");
            return;
        }

        //2) 스킬 대상 플래그 초기화
        isEnemyAll = false;
        isTeamAll = false;

        //3) 스킬 대상 지정
        SetTarget(skill, character, ref target);

        // 3-1) SetTarget에서 지정 안 된 경우, 기본값(전방 첫 적) 지정
        if (target == null)
        {
            var enemies = BattleManager.instance.enemyList;
            if (enemies.Count > 0)
                target = enemies[0].GetComponent<PlayerController>();
            else
            {
                Debug.LogWarning("적이 없어서 스킬 대상을 찾을 수 없습니다.");
                return;
            }
        }

        // 4) 실제 처리할 대상 리스트 구성
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

        // 5) 스킬 적용: try-finally로 플래그 무조건 초기화
        try
        {
            // ───────── A 효과 적용 ─────────
            foreach (var t in targetsA)
            {
                ApplySingleEffect(skill.typeA, skill, character, t , false);
                SetEffect(skill, character, t);
            }

            // ───────── B 효과 적용 ─────────
            if (!string.IsNullOrEmpty(skill.typeB) && skill.typeB != "0")
            {
                // 플래그 초기화
                isEnemyAll = false;
                isTeamAll = false;
                // 대상 재지정
                PlayerController targetB = target;
                SetTargetB(skill, character, ref targetB);

                // 대상 리스트 구성
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
            // 플래그 누락 없이 무조건 리셋
            isEnemyAll = false;
            isTeamAll = false;
        }


        //다 끝나고 말풍선
        PoolManager.Instance.SpawnEffectWithText
            ("Balloon", character.transform.position, Quaternion.identity, skill.name);
    }

    //스킬 준비부
    #region Set Skill

    void SetTargetB(SkillData skill, PlayerController caster, ref PlayerController target)
    {
        // (1) decoy 우선 처리는 이미 UseSkill 상위에서 끝났으니 생략
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
                Debug.LogWarning($"알 수 없는 targetB 값 '{skill.targetB}'.");
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
                Debug.LogError("스킬 사이즈값이 잘못 입력됨");
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
                Debug.LogError("스킬 사이즈값이 잘못 입력됨");
                return 0;
        }
    }
    public void SetTarget(SkillData skill, PlayerController character, ref PlayerController target)
    {
        // ────────────────────────────────────
        // 1) 디코이 효과 우선 처리
        //    (caster.isEnemy에 따라 적 또는 아군 리스트 중에서)
        var list = character.isEnemy
            ? BattleManager.instance.playerList   // 적이면 플레이어리스트
            : BattleManager.instance.enemyList;   // 아군이면 적 리스트

        var decoyPc = list
            .Select(go => go.GetComponent<PlayerController>())
            .FirstOrDefault(pc => pc.HasEffect(StatusEffect.Type.Decoy));

        if (decoyPc != null)
        {
            target = decoyPc;
            return;   // 디코이 대상으로 운명 결정!
        }
        // ────────────────────────────────────

        List<GameObject> el = BattleManager.instance.enemyList;
        List<GameObject> pl = BattleManager.instance.playerList;
        switch (skill.targetA)
        {
            case "enemy"://전방의 적 하나
                if (el.Count > 0)
                    target = el[0].GetComponent<PlayerController>();
                break;
            case "enemyAll"://적 전체
                isEnemyAll = true;
                break;
            case "enemy5"://최후방의 적 하나
                target = el[el.Count - 1].GetComponent<PlayerController>();
                break;
            case "enemy4"://후방의 적 하나
                if(el.Count > 2) target = el[el.Count - 2].GetComponent<PlayerController>();
                else target = el[el.Count - 1].GetComponent<PlayerController>();
                break;
            case "me"://자신
                target = character;
                break;
            case "teamAll"://아군 전체
                isTeamAll = true;
                break;
            case "team1"://최전방의 아군
                target = pl[0].GetComponent<PlayerController>();
                break;
            case "team5"://최후방의 아군
                target = pl[pl.Count - 1].GetComponent<PlayerController>();
                break;
            case "teamHp"://체력이 가장 적은 아군
                var weakest = pl
                    .Select(go => go.GetComponent<PlayerController>())
                    .Where(pc => pc != null)
                    .OrderBy(pc => pc.curHP / pc.characterData.hp)
                    .FirstOrDefault();
                if (weakest != null)
                    target = weakest;
                break;
            case "teamMp"://마력이 가장 적은 아군
                GameObject minMp = pl.OrderBy(x => x.GetComponent<PlayerController>().curMP).FirstOrDefault();
                target = minMp.GetComponent<PlayerController>();
                break;
            default:
                Debug.LogWarning($"알 수 없는 targetA 값 '{skill.targetA}'.");
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
        // 공백 제거까지 확실히
        string cast = skill.castEffect?.Trim() ?? "";
        string hit = skill.hitEffect?.Trim() ?? "";

        // (1) 디버그로 문자열 상태 확인해보기
        Debug.Log($"[SetEffect] cast:'{cast}', hit:'{hit}'");

        //주문자
        if (cast != "0")
            PoolManager.Instance.SpawnEffect(cast, character.transform.position, Quaternion.identity);

        //피주문자
        if (hit != "0")
            PoolManager.Instance.SpawnEffect(hit, target.transform.position, Quaternion.identity);
    }
    #endregion

    //스킬 사용부
    #region Use Skill

    void ApplySingleEffect(string typeKey, SkillData skill, PlayerController caster, PlayerController target, bool isB)
    {
        // “duration”을 isB 에 따라 골라서 계산
        float duration = isB ? skill.timeB : skill.timeA;

        switch (typeKey)
        {
            // 물리/마법 공격
            case "atk":
                UseAtk(skill, caster, target);
                break;
            case "mAtk":
                UseMAtk(skill, caster, target);
                break;

            // 버프
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

            // 디버프
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

            // 회복
            case "healHp":
                UseHealHP(skill, caster, target);
                break;
            case "healMp":
                UseHealMP(skill, caster, target);
                break;

            // 상태 이상
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
        //회피 판정
        if (CheckIsMissed(target))
        {
            PoolManager.Instance.SpawnEffectWithText
                ("Miss", target.transform.position, Quaternion.identity, "Miss!");
            return;
        }

        //데미지 계산
        float damageSize = ConvertSizeToFloat(skill.sizeA);
        float levelDamage = SetLevelDamage(skill.sizeA);
        float damage = damageSize * character.curAtk + character.characterData.lv * levelDamage;
        float finalDamage = damage / (1 + target.curDef / 100);

        // 크리티컬 판정
        bool isCrit = Random.value < character.curCri / 100f;
        if (isCrit)
        {
            finalDamage *= 1.5f; // 크리 배율, 예: 1.5배
            Vector3 newPos = target.transform.position;
            newPos.y += 1f;
            PoolManager.Instance.SpawnEffectWithText
                ("DamageA", newPos, Quaternion.identity, "Critical!");
        }

        target.curHP -= finalDamage;

        //스킬,데미지 이펙트
        PoolManager.Instance.SpawnEffectWithText
            ("DamageA", target.transform.position, Quaternion.identity, Mathf.Round(finalDamage).ToString());

        Debug.Log(character.name + "(이)가 " + target.name + "에게 " + finalDamage + "데미지!");

    }

    void UseMAtk(SkillData skill, PlayerController character, PlayerController target)
    {
        //회피 판정
        if (CheckIsMissed(target))
        {
            PoolManager.Instance.SpawnEffectWithText
                ("Miss", target.transform.position, Quaternion.identity, "Miss!");
            return;
        }

        //데미지 계산
        float damageSize = ConvertSizeToFloat(skill.sizeA);
        float levelDamage = SetLevelDamage(skill.sizeA);
        float damage = damageSize * character.curMAtk + character.characterData.lv * levelDamage;
        float finalDamage = damage / (1 + target.curMDef / 100);

        // 크리티컬 판정
        bool isCrit = Random.value < character.curCri / 100f;
        if (isCrit)
        {
            finalDamage *= 1.5f; // 크리 배율, 예: 1.5배
            Vector3 newPos = target.transform.position;
            newPos.y += 0.5f;
            PoolManager.Instance.SpawnEffectWithText
                ("DamageM", newPos, Quaternion.identity, "Critical!");
        }

        target.curHP -= finalDamage;

        //스킬,데미지 이펙트
        PoolManager.Instance.SpawnEffectWithText
            ("DamageM", target.transform.position, Quaternion.identity, Mathf.Round(finalDamage).ToString());

        Debug.Log(character.name + "(이)가 " + target.name + "에게 " + finalDamage + "데미지!");
    }

    void UseBuff(SkillData skill, PlayerController caster, PlayerController t, 
        float buffStat, StatusEffect.Type effectType, float duration)
    {
        // 1) 퍼센트 포인트 계산
        float levelPoint = SetLevelDamage(skill.sizeA);        // s=8, m=20, …
        float levelPointBonus = caster.characterData.lv;            // 레벨만큼
        float buffPercent = (levelPoint + levelPointBonus) / 100f;

        // 2) 실제 버프량
        float buffAmount = buffStat * buffPercent;

        // 3) 적용
        t.ApplyEffect(effectType, buffAmount, duration);

        Debug.Log($"{caster.characterData.name} → {t.characterData.name}에게 " +
                  $"{effectType}을 {skill.sizeA}만큼 적용: +{buffPercent * 100f:F1}% (+{buffAmount:F1}), " +
                  $"지속시간 {duration}s");
    }


    void UseHealHP(SkillData skill, PlayerController caster, PlayerController target)
    {
        if (target == null) return;  // 안전장치

        // 1) 힐량 계산
        float healSize = ConvertSizeToFloat(skill.sizeA);   // 예: s,m,l, xl에 대응하는 계수
        float levelHeal = SetLevelDamage(skill.sizeA);       // 스킬 레벨별 고정치

        float healAmount = target.characterData.hp * healSize / 10 + caster.characterData.lv * levelHeal / 100;

        // 2) 대상의 체력에 적용 & 최대치 캡
        target.curHP += healAmount;
        if (target.curHP > target.characterData.hp)
            target.curHP = target.characterData.hp;

        // 3) 이펙트 및 로그
        PoolManager.Instance.SpawnEffectWithText(
            "Heal", target.transform.position, Quaternion.identity,
            "+" + Mathf.RoundToInt(healAmount).ToString()
        );
        Debug.Log($"{caster.characterData.name}이(가) {target.characterData.name}에게 {Mathf.RoundToInt(healAmount)}만큼 치유!");
    }
    void UseHealMP(SkillData skill, PlayerController caster, PlayerController target)
    {
        if (target == null) return;  // 안전장치

        // 1) 힐량 계산
        float healSize = ConvertSizeToFloat(skill.sizeA);   // 예: s,m,l, xl에 대응하는 계수
        float levelHeal = SetLevelDamage(skill.sizeA);       // 스킬 레벨별 고정치

        float healAmount = healSize * 10 + caster.characterData.lv * levelHeal / 100;

        // 2) 대상의 체력에 적용 & 최대치 캡
        target.curMP += healAmount;
        if (target.curMP > 100)
            target.curMP = 100;

        // 3) 이펙트 및 로그
        PoolManager.Instance.SpawnEffectWithText(
            "Heal", target.transform.position, Quaternion.identity,
            "+" + Mathf.RoundToInt(healAmount).ToString()
        );
        Debug.Log($"{caster.characterData.name}이(가) {target.characterData.name}에게 {Mathf.RoundToInt(healAmount)}만큼 마력 회복!");
    }

    void UseStun(SkillData skill, PlayerController target, float duration)
    {
        target.ApplyEffect(StatusEffect.Type.Stun, 0f, duration);
        Debug.Log($"{target.name}이 {duration}초 동안 스턴!");
    }
    void UseBurn(SkillData skill, PlayerController target, float duration)
    {
        if (target == null) return;
        // 코루틴 시작
        target.ApplyEffect(StatusEffect.Type.Burn, 0, duration);
        StartCoroutine(BurnCoroutine(target, duration));
    }
    //0.5초마다 현재 체력의 3% 데미지
    private IEnumerator BurnCoroutine(PlayerController target, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && target.curHP > 0f)
        {
            if (target == null) yield break;
            yield return new WaitForSeconds(0.5f);
            // 현재 체력의 3% 계산
            float damage = target.curHP * 0.03f;
            target.curHP -= damage;

            // 이펙트 + 로그
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
        Debug.Log($"{caster.characterData.name}에게 {duration}초 동안 디코이 상태 부여!");  // 확인용
    }
    void UseCleanse(PlayerController target)
    {
        // 디버프만 전부 제거
        var sem = target.GetComponent<StatusEffectManager>();
        sem.RemoveAll(eff => eff.type.ToString().StartsWith("Debuff"));
    }

    // 20% 확률로 target을 즉사(=현재 HP만큼 데미지), 실패 시 caster가 사망.
    void UseJoke(SkillData skill, PlayerController caster, PlayerController target)
    {
        if (target == null || caster == null) return;

        // 성공 확률 20%
        bool success = Random.value < 0.2f;

        if (success)
        {
            // target 즉사
            float damage = target.curHP;
            target.curHP = 0f;
            PoolManager.Instance.SpawnEffectWithText(
                "DamageA",
                target.transform.position,
                Quaternion.identity,
                $"{Mathf.RoundToInt(damage)}!"
            );
            Debug.Log($"{caster.characterData.name}의 joke 스킬 성공! {target.characterData.name} 즉사!");
        }
        else
        {
            // caster 사망
            float selfDamage = caster.curHP;
            caster.curHP = 0f;
            PoolManager.Instance.SpawnEffectWithText(
                "DamageA",
                caster.transform.position,
                Quaternion.identity,
                $"{Mathf.RoundToInt(selfDamage)}!"
            );
            Debug.Log($"{caster.characterData.name}의 joke 스킬 실패... 자신이 사망.");
        }
    }
    #endregion
}
