using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PyroPassive3 : BaseSkill
{
	public override DamageWithType PassiveSkillEffect(CombatActor user, DamageTypes damageType, float damageNumber, CombatActor enemy)
	{
		DamageWithType damageReturn = new();
		
		var dmgNumber = damageNumber += 3;
		
		damageReturn.dmgType = damageType;
		damageReturn.dmgNumber =  dmgNumber;
		
		return (damageReturn);
	}

	public override bool PassiveIsLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
	{
		return enemy.debuffList.FirstOrDefault(x => x.InflictedDebuff == debuffTypes.Burn ) != null
		       && dmgWithType.dmgType is DamageTypes.Fire;
	}
}
