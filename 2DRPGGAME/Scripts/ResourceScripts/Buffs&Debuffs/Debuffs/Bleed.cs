using Godot;
using System;

public partial class Bleed : Debuff
{
    public override void Apply(CombatActor target)
    {
        if (target.statusEffectList.Contains(this)) return;

        target.statusEffectList.Add((Bleed)Duplicate(true));
    }

    public override bool StatusEffect(CombatActor target)
    {
        target.TakeDamage(DebuffDamage);
        Duration--;
        GD.Print($"-1 Duration\nCurrent duration:{Duration}");
        return true;
    }
    
}