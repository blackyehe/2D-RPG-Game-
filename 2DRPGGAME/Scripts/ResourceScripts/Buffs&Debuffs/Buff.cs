using Godot;
using System;
using NewGameProject.Scripts;

public abstract partial class Buff : StatusEffectBase, IApplicable, IStatusEffect
{
    [Export] public buffTypes BestowedBuff;
    public int StackCount = 1;
    public abstract void Apply(CombatActor target);
    public abstract bool TriggerStatusEffect(CombatActor target);
    public abstract bool IsStatusPassiveLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy);
    public abstract DamageWithType StatusPassive(CombatActor user, DamageWithType dmgWithType, CombatActor enemy);

    public override void RemoveFromList(CombatActor target)
    {
        target.buffList.Remove(this);
    }
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
}
