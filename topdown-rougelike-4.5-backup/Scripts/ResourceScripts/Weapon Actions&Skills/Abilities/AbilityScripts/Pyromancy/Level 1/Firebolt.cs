using Godot;
using System;

public partial class Firebolt : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		double damage = SkillDamage;
		target.TakeDamage(damage);
		user.RemoveCostAfterAction();
	}
	
	public Firebolt()
	{
		SkillAnimation = AnimTags.FireSpellCasting;
	}
}
