using Godot;
using System;

public partial class Slash : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		var damageValues = user.GetActiveWeaponBySlotType(EquipSlot.MainHand).weaponResource.DamageByDistribution();

		user.DamageToDealAfterCalc(damageValues, user, target);

		user.RemoveCostAfterAction();
	}
	
	public Slash()
	{
		//animationDuration = abilityUser.Stats.LightAttackAnimDuration;
		SkillAnimation = AnimTags.LightAttack;
	}
}
