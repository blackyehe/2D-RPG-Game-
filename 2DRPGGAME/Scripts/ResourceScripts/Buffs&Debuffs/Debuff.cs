using Godot;
using System;
using NewGameProject.Scripts;

public abstract partial class Debuff : StatusEffectBase, IApplicable, IStatusEffect
{
    [Export] public debuffTypes InflictedDebuff;
    [Export] public float DebuffDamage;
    public int StackCount = 1;
    public abstract void Apply(CombatActor target);
    public abstract bool TriggerStatusEffect(CombatActor target);
    public abstract bool IsStatusPassiveLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy);
    public abstract DamageWithType StatusPassive(CombatActor user, DamageWithType dmgWithType, CombatActor enemy);
    public bool CheckDuration(CombatActor target, double duration, int stackCount)
    {
        if (CurrentDuration <= 0 && StackCount <= 1)
        {
            target.statusEffectList.Remove(this);
            RemoveFromList(target);
            return true;
        }
        return false;
    }

    public override void RemoveFromList(CombatActor target)
    {
        target.debuffList.Remove(this);
    }
}
