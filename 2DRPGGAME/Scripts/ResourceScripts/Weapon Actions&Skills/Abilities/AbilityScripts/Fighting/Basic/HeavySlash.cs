namespace NewGameProject.Scripts.Resources.Weapon_Actions_Skills.Abilities.AbilityScripts.MeleeScript;

public partial class HeavySlash : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		var damageValues = user.GetActiveWeaponBySlotType(EquipSlot.MainHand).weaponResource.DamageByDistribution();

		user.DamageToDealAfterCalc(damageValues, user, target);

		user.RemoveCostAfterAction();
	}
	
	public HeavySlash()
	{
		//animationDuration = abilityUser.Stats.LightAttackAnimDuration;
		SkillAnimation = AnimTags.HeavyAttack;
	}
}