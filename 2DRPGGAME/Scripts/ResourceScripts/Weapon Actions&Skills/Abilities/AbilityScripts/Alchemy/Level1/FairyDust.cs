using Godot;
using System;

public partial class FairyDust : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		//single target buff, gives the target weak hp regen and weak mana regen, also removes poison and paralyze
	}

	public override double GetAnimDuration(CombatActor user)
	{
		throw new NotImplementedException();
	}
}
