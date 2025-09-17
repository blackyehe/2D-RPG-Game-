using Godot;
using System;

public abstract partial class BaseWeapon : EquipableItem
{
    [Export] Area2D area2D;
    [Export] CollisionShape2D collisionShape2D;
    public WeaponResource weaponResource;

    public WeaponType WeaponType;
    public WeaponProperties[] WeaponProperties;

    public override void _Ready()
    {
        area2D.BodyEntered += Area2D_BodyEntered;
        weaponResource = (WeaponResource)ItemResource;
    }

    public override void OnEquip(CombatActor itemUser)
    {
        if (weaponResource.WeaponType is WeaponType.Melee)
        {
            itemUser.Stats.MaxAttack += weaponResource.WeaponDamage;
        }
    }

    public override void OnUnequip(CombatActor itemUser)
    {
        if (weaponResource.WeaponType is WeaponType.Melee)
        {
            itemUser.Stats.MaxAttack -= weaponResource.WeaponDamage;
        }
    }

    private void Area2D_BodyEntered(Node2D body)
    {
        if (body is not Player player || player.inventory.TestItemCount == player.inventory.inventorySize)
            return;
        player.inventory.AddItem(this);
        collisionShape2D.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
        Visible = false;
    }
}