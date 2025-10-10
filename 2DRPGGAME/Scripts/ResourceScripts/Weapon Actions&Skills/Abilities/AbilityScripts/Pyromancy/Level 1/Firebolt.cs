using Godot;
using System;

public partial class Firebolt : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		double damage = user.GetWeaponDMGBySlotType(EquipSlot.Ranged);
		target.TakeDamage(damage);
		user.RemoveCostAfterAction();
	}

	public override double GetAnimDuration(CombatActor user)
	{
		throw new NotImplementedException();
	}
}
