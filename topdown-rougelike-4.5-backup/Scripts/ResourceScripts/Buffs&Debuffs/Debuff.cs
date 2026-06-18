using Godot;
using System;
using NewGameProject.Scripts;

public abstract partial class Debuff : StatusEffectBase, IApplicable, IStatusEffect
{
    [Export] public debuffTypes InflictedDebuff;
    [Export] public double DebuffDamage;
    public abstract void Apply(CombatActor target);
    public abstract bool StatusEffect(CombatActor target);
}
