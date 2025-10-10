using Godot;
using System;

public partial class PoisonBreath : WeaponBaseAction
{
	public override void DoAction(CombatActor user, CombatActor target)
	{
		//cone shape, has a chance to apply poison to the target
	}

	public override double GetAnimDuration(CombatActor user)
	{
		throw new NotImplementedException();
	}
}
