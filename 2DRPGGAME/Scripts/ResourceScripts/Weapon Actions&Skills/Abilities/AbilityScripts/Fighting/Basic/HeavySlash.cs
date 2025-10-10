namespace NewGameProject.Scripts.Resources.Weapon_Actions_Skills.Abilities.AbilityScripts.MeleeScript;

public partial class HeavySlash : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		double damage = user.GetWeaponDMGBySlotType(EquipSlot.MainHand);
		target.TakeDamage(damage);
		user.RemoveCostAfterAction();
	}
	public override double GetAnimDuration(CombatActor user)
	{
		return user.Stats.HeavyAttackAnimDuration;
	}

	public HeavySlash()
	{
		//animationDuration = abilityUser.Stats.LightAttackAnimDuration;
		SkillAnimation = AnimTags.HeavyAttack;
	}
}