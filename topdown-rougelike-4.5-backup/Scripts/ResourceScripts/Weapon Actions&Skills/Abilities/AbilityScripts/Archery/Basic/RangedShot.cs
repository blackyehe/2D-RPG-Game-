	using Godot;
using System;

public partial class RangedShot : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		double damage = user.GetWeaponDMGBySlotType(EquipSlot.Ranged);
		target.TakeDamage(damage);
		user.RemoveCostAfterAction();
	}
	
	public RangedShot()
	{
		//animationDuration = abilityUser.Stats.RangedShotAnimDuration;
		SkillAnimation = AnimTags.BowAttack;
	}
}
