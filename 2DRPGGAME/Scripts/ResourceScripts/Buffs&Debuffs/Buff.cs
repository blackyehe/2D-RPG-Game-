using Godot;
using System;

public abstract partial class Buff : StatusEffectBase
{
    [Export] public buffTypes BestowedBuff;
    public abstract void Apply(CombatActor target);
    public abstract void BuffEffect(CombatActor target);
}
