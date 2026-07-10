using Godot;
using System;
using Godot.Collections;

public partial class PyroPassive2 : BaseSkill
{
	public override DamageWithType PassiveSkillEffect(CombatActor user, DamageTypes damageType, float damageNumber, CombatActor enemy)
	{
		throw new NotImplementedException();
	}

	public override bool PassiveIsLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy)
	{
		throw new NotImplementedException();
	}
}
