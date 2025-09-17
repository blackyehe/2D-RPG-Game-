using Godot;
using System;

public abstract partial class Debuff : StatusEffectBase
{
    [Export] public debuffTypes InflictedDebuff;
    [Export] public double DebuffDamage;
    public abstract void Apply(CombatActor target);
    public abstract void DebuffEffect(CombatActor target);
}
