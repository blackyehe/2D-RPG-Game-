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

	public override double GetAnimDuration(CombatActor user)
	{
		return user.Stats.RangedShotAnimDuration;
	}

	public RangedShot()
	{
		//animationDuration = abilityUser.Stats.RangedShotAnimDuration;
		SkillAnimation = AnimTags.BowAttack;
	}
}
