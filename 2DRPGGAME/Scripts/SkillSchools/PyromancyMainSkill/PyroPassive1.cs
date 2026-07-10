using Godot;
using System;
using Godot.Collections;

public partial class PyroPassive1 : BaseSkill
{
	public override DamageWithType PassiveSkillEffect(CombatActor user, DamageTypes damageType,float damageNumber, CombatActor enemy)
	{
		float dmgNumber = damageNumber + 3;
		
		DamageWithType damageReturn = new();
		
		damageReturn.dmgType = damageType;
		damageReturn.dmgNumber = dmgNumber;
		
		return (damageReturn);
	}

	public override bool PassiveIsLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
	{
		return dmgWithType.dmgType == DamageTypes.Fire;
	}
}
