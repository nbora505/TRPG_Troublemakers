using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectManager : MonoBehaviour
{
    [Header("아이콘 설정")]
    public Sprite buffAtkIcon;
    public Sprite buffDefIcon;
    public Sprite buffMAtkIcon;
    public Sprite buffMDefIcon;
    public Sprite buffCriIcon;
    public Sprite buffMissIcon;
    public Sprite debuffAtkIcon;
    public Sprite debuffDefIcon;
    public Sprite debuffMAtkIcon;
    public Sprite debuffMDefIcon;
    public Sprite debuffCriIcon;
    public Sprite debuffMissIcon;
    public Sprite decoyIcon;
    public Sprite stunIcon;
    public Sprite burnIcon;
    // 필요에 따라 더 추가...

    public Transform iconContainer;    // GridLayoutGroup 붙은 빈 오브젝트
    public GameObject iconPrefab;      // Image 하나 가진 프리팹

    private List<StatusEffect> effects = new List<StatusEffect>();
    private Dictionary<StatusEffect, GameObject> iconMap = new();

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            var eff = effects[i];
            eff.duration -= dt;
            if (eff.duration <= 0f)
                RemoveEffect(eff);
        }
    }

    public void AddEffect(StatusEffect newEff)
    {
        effects.Add(newEff);

        // 스탯 바로 적용
        ApplyStatChange(newEff, +newEff.amount);

        // 아이콘 생성
        GameObject go = Instantiate(iconPrefab, iconContainer);
        go.GetComponent<Image>().sprite = GetIconForType(newEff.type);
        iconMap[newEff] = go;
    }

    public void RemoveEffect(StatusEffect eff)
    {
        // 스탯 원복
        ApplyStatChange(eff, -eff.amount);

        // 아이콘 삭제
        if (iconMap.TryGetValue(eff, out var go))
        {
            Destroy(go);
            iconMap.Remove(eff);
        }

        effects.Remove(eff);
    }

    Sprite GetIconForType(StatusEffect.Type type)
    {
        switch (type)
        {
            case StatusEffect.Type.BuffAtk: return buffAtkIcon;
            case StatusEffect.Type.BuffDef: return buffDefIcon;
            case StatusEffect.Type.BuffMAtk: return buffMAtkIcon;
            case StatusEffect.Type.BuffMDef: return buffMDefIcon;
            case StatusEffect.Type.BuffMiss: return buffMissIcon;
            case StatusEffect.Type.BuffCri: return buffCriIcon;

            case StatusEffect.Type.DebuffAtk: return debuffAtkIcon;
            case StatusEffect.Type.DebuffDef: return debuffDefIcon;
            case StatusEffect.Type.DebuffMAtk: return debuffMAtkIcon;
            case StatusEffect.Type.DebuffMDef: return debuffMDefIcon;
            case StatusEffect.Type.DebuffMiss: return debuffMissIcon;
            case StatusEffect.Type.DebuffCri: return debuffCriIcon;

            case StatusEffect.Type.Stun: return stunIcon;
            case StatusEffect.Type.Burn: return burnIcon;
            case StatusEffect.Type.Decoy: return decoyIcon;
            default: return null;
        }
    }

    void ApplyStatChange(StatusEffect eff, float delta)
    {
        var pc = GetComponent<PlayerController>();
        switch (eff.type)
        {
            //버프
            case StatusEffect.Type.BuffAtk:
                pc.curAtk += delta;
                break;
            case StatusEffect.Type.BuffDef:
                pc.curDef += delta;
                break;
            case StatusEffect.Type.BuffMAtk:
                pc.curMAtk += delta;
                break;
            case StatusEffect.Type.BuffMDef: 
                pc.curMDef += delta;
                break;

            //디버프
            case StatusEffect.Type.DebuffAtk:
                pc.curAtk -= delta;
                break;
            case StatusEffect.Type.DebuffDef:
                pc.curDef -= delta;
                break;
            case StatusEffect.Type.DebuffMAtk: 
                pc.curMAtk -= delta;
                break;
            case StatusEffect.Type.DebuffMDef: 
                pc.curMDef -= delta;
                break;

            //상태 이상
            case StatusEffect.Type.Stun:
                //StartCoroutine(pc.StunCoroutine(eff.duration));
                break;
            case StatusEffect.Type.Burn:
                break;
            case StatusEffect.Type.Decoy: 
                break;
        }
    }

    public void RemoveAll(System.Predicate<StatusEffect> match)
    {
        // 뒤에서부터 순회하며 제거
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            var eff = effects[i];
            if (match(eff))
                RemoveEffect(eff);
        }
    }
    public bool HasEffect(StatusEffect.Type type)
    {
        return effects.Exists(eff => eff.type == type);
    }

}
