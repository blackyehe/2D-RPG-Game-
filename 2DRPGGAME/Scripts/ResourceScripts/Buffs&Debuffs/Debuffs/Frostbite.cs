using Godot;
using System;

public partial class Frostbite : Debuff
{
	public override bool TriggerStatusEffect(CombatActor target)
	{
		CheckDuration(target, CurrentDuration, StackCount);

		target.TakeDamage(DebuffDamage);
		CurrentDuration--;
		
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
