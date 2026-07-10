using Godot;
using System;
using System.Collections.Generic;

public partial class Firebolt : WeaponBaseAction
{
    public override void DoAction(CombatActor user, CombatActor target)
    {
        var damageValues = SkillDamageByDistribution();

        user.DamageToDealAfterCalc(damageValues, user, target);

        user.RemoveCostAfterAction();
    }

    public Firebolt()
    {
        SkillAnimation = AnimTags.FireSpellCasting;
    }
}