using Godot;
using System;
using System.Linq;

public partial class Slash : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		var damageValues = WeaponAndSkillDistributionForDmgCalc(user, EquipSlot.MainHand);

		user.DamageToDealAfterCalc(damageValues, user, target);

		user.RemoveCostAfterAction();
	}
	
	public Slash()
	{
		//animationDuration = abilityUser.Stats.LightAttackAnimDuration;
		SkillAnimation = AnimTags.LightAttack;
	}
}
