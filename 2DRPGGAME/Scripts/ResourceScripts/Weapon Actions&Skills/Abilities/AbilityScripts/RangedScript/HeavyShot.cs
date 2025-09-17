using Godot;
using System;

public partial class HeavyShot : WeaponBaseAction

{
    public override void DoAction(CombatActor user, CombatActor target)
    {
        double damage = user.GetWeaponDMGBySlotType(EquipSlot.Ranged);
        target.TakeDamage(damage);
        user.RemoveCostAfterAction();
    }

    public override double GetAnimDuration(CombatActor user)
    {
        return user.Stats.RangedShotAnimDuration;
    }

    public HeavyShot()
    {
        //animationDuration = abilityUser.Stats.RangedShotAnimDuration;
        SkillAnimation = AnimTags.BowAttack;
    }
}