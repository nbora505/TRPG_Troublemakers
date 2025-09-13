using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public enum Type { 
        BuffAtk, BuffDef, BuffMAtk, BuffMDef, BuffCri, BuffMiss,
        DebuffAtk, DebuffDef, DebuffMAtk, DebuffMDef, DebuffCri, DebuffMiss,
        Stun, Burn, Decoy
    }
    public Type type;
    public float amount;
    public float duration;
    public Sprite iconSprite;

    public StatusEffect(Type type, float amount, float duration)
    {
        this.type = type;
        this.amount = amount;
        this.duration = duration;
    }
}

