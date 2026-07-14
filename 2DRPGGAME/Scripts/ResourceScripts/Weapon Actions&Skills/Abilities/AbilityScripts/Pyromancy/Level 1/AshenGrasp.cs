using Godot;
using System;

public partial class AshenGrasp : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		var damageValues = SkillDamageByDistributionForDmgCalc();

		user.DamageToDealAfterCalc(damageValues, user, target);

		user.RemoveCostAfterAction();
	}

	public AshenGrasp()
	{
		SkillAnimation = AnimTags.FireSpellCasting;
	}
}
