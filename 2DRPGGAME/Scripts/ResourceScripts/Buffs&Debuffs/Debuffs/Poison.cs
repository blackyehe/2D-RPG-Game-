using Godot;
using System;

public partial class Poison : Debuff
{
	public override void Apply(CombatActor target)
	{
		if (!target.statusEffectList.Contains(this))
		{
			target.statusEffectList.Add(this);
			target.debuffList.Add(this);
		}
		else
		{
			StackCount++;
		}
		CurrentDuration = Duration;
	}

	public override bool TriggerStatusEffect(CombatActor target)
	{
		CheckDuration(target, CurrentDuration, StackCount);
		
		target.TakeDamage(DebuffDamage + StackCount);
		CurrentDuration--;
		StackCount--;
		
		CheckDuration(target, CurrentDuration, StackCount);
		return true;
	}

	public override bool IsStatusPassiveLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
	{
		throw new NotImplementedException();
	}

	public override DamageWithType StatusPassive(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
	{
		throw new NotImplementedException();
	}
}
