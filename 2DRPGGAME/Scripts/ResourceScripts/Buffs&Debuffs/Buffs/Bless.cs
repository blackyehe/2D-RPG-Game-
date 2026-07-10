using Godot;
using System;

public partial class Bless : Buff
{
    public override void Apply(CombatActor target)
    {
        CurrentDuration = Duration;
        if (target.statusEffectList.Contains(this)) return;

        target.statusEffectList.Add(this);
        target.buffList.Add(this);
    }

    public override bool TriggerStatusEffect(CombatActor target)
    {
        throw new NotImplementedException();
    }

    public override bool IsStatusPassiveLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
    {
        return !user.IsTurnActive && user.buffList.Contains(this);
    }

    public override DamageWithType StatusPassive(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
    {
        var damage = dmgWithType;
        damage.dmgNumber = damage.dmgNumber -= 5;
        damage.dmgType = dmgWithType.dmgType;

        return damage;
    }
}