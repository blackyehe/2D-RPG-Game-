using Godot;
using System;

public partial class Cauterize : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		//Heals the target for a small amount and removes bleed but reduces movement speed slightly.
	}

	public override double GetAnimDuration(CombatActor user)
	{
		throw new NotImplementedException();
	}
}
