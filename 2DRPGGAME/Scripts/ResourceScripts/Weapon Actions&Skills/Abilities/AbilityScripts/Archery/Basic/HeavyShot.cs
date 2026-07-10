using Godot;
using System;

public partial class HeavyShot : WeaponBaseAction

{
    public override void DoAction(CombatActor user, CombatActor target)
    {
       float damage = user.GetWeaponDMGBySlotType(EquipSlot.Ranged);
        target.TakeDamage(damage);
        user.RemoveCostAfterAction();
    }

    public HeavyShot()
    {
        //animationDuration = abilityUser.Stats.RangedShotAnimDuration;
        SkillAnimation = AnimTags.BowAttack;
    }
}