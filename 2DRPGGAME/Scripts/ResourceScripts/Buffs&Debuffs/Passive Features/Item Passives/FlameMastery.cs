using Godot;
using System;

public partial class FlameMastery : PassiveFeature
{
	public override bool IsFeatureEffectLegal(CombatActor user, DamageWithType damage, CombatActor target)
	{
		return damage.dmgType is DamageTypes.Fire;
	}

	public override void PassiveFeatureEffect(CombatActor user, DamageWithType damage, CombatActor target)
	{
		if (!user.InCombat) throw new NotImplementedException();
		AppliedStatusEffect.Apply(user,AppliedStatusEffect);
	}
}
