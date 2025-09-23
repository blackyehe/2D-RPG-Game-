using Godot;
using System;
using NewGameProject.Scripts;

public abstract partial class Buff : StatusEffectBase, IApplicable, IStatusEffect
{
    [Export] public buffTypes BestowedBuff;
    public abstract void Apply(CombatActor target);
    public abstract bool StatusEffect(CombatActor target);
}
