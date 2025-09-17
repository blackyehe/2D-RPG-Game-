using Godot;
using System;

public partial class Poison : Debuff
{
	public override void Apply(CombatActor target)
	{
		target.statusEffectList.Add(this);
	}

	public override void DebuffEffect(CombatActor target)
	{
        
		if (Duration <= 0) target.statusEffectList.Remove(this);
        
		else target.TakeDamage(DebuffDamage);
		Duration--;
	}
}
