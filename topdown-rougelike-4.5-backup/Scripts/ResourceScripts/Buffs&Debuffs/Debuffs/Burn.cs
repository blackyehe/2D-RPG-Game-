using Godot;
using System;

public partial class Burn : Debuff
{
	public override void Apply(CombatActor target)
	{
		target.statusEffectList.Add(this);
	}

	public override bool StatusEffect(CombatActor target)
	{
        
		if (Duration <= 0)
		{
			target.statusEffectList.Remove(this);
			return true;
		}

		target.TakeDamage(DebuffDamage);
		Duration--;
		return true;
	}
}
