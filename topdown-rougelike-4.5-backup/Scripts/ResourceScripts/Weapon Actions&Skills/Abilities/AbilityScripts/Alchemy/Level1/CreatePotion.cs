using Godot;
using System;

public partial class CreatePotion : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		//Can create a wider variety of potions from ingredients collected or using mana as a substitute depending on
		// the level of Alchemy the user is proficient in. In combat it costs a bonus action to create. 
		//At level 1 Healing, Mana and Poison potions are available.
	}

	
}
