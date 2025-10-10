using Godot;
using System;

public partial class Slash : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		double damage = user.GetWeaponDMGBySlotType(EquipSlot.MainHand);
		target.TakeDamage(damage);
		user.RemoveCostAfterAction();
	}

	public override double GetAnimDuration(CombatActor user)
	{
		return user.Stats.LightAttackAnimDuration;
	}

	public Slash()
	{
		//animationDuration = abilityUser.Stats.LightAttackAnimDuration;
		SkillAnimation = AnimTags.LightAttack;
	}
}
