using Godot;
using System;
using System.Collections.Generic;

public partial class FireBall : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		var targetPos = TurnManager.Instance.GetTilePosition(target.GlobalPosition);
		var enemyPosList = TurnManager.Instance.GetAllEnemiesPos();
		var enemyList = new List<CombatActor>();
		
		foreach (var enemyPos in enemyPosList)
		{
			if (targetPos.DistanceTo(enemyPos)<=3)
			{
				enemyList.Add(TurnManager.Instance.GetEnemyTile(enemyPos));
			}
		}

		foreach (var enemy in enemyList)
		{
			var damageValues = SkillDamageByDistributionForDmgCalc();

			user.DamageToDealAfterCalc(damageValues, user, target);

			user.RemoveCostAfterAction();
		}
	}
	
	public FireBall()
	{
		SkillAnimation = AnimTags.FireSpellCasting;
	}
}
