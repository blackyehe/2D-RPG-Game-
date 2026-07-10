using Godot;
using System;

public partial class ArcaneOverflow : Buff
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
		if (!user.buffList.Contains(this)) return false;
		
		var type =  dmgWithType.dmgType;
		
		return type != DamageTypes.Bludgeoning && type != DamageTypes.Slashing && type != DamageTypes.Piercing;
	}

	public override DamageWithType StatusPassive(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
	{
		var damage =  dmgWithType;
		damage.dmgNumber = damage.dmgNumber += 1;
		damage.dmgType = dmgWithType.dmgType;
		
		return damage;
	}
}
