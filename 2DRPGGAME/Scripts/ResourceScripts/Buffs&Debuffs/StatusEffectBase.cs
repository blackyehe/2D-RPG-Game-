using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public abstract partial class StatusEffectBase : Resource
{
    [Export] public Texture2D Sprite;
    [Export] public string Name;
    [Export] public string Description;
    [Export] public double Duration;
    [Export] public bool IsStackable;
    [Export] public int MaxStackCount;
    public double CurrentDuration;
    public int StackCount = 1;

    public void Apply(CombatActor target, StatusEffectBase statusEffect)
    {
        if (target.statusEffectList.Contains(statusEffect))
        {
            CurrentDuration = statusEffect.Duration += CurrentDuration;
            if (CurrentDuration > MaxStackCount) CurrentDuration = MaxStackCount;
            if (IsStackable) StackCount++;
            return;
        }

        if (!target.statusEffectList.Contains(statusEffect))
        {
            switch (statusEffect)
            {
                case Debuff debuff:
                    target.debuffList.Add(debuff);
                    break;
                case Buff buff:
                    target.buffList.Add(buff);
                    break;
            }

            target.statusEffectList.Add(statusEffect);
            StackCount = 1;
            CurrentDuration = statusEffect.Duration;
        }
    }

    public abstract void RemoveFromList(CombatActor target);

    public static void RemoveStatus(CombatActor target)
    {
        for (int i = 0; i < target.statusEffectList.Count; i++)
        {
            if (!(target.statusEffectList[i].CurrentDuration <= 0)) continue;

            if (target.statusEffectList[i] is Debuff debuff) debuff.RemoveFromList(target);
            if (target.statusEffectList[i] is Buff buff) buff.RemoveFromList(target);

            target.statusEffectList.Remove(target.statusEffectList[i]);

            i--;
        }
    }
}