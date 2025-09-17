using Godot;
using System;

public abstract partial class EquipableItem : Item
{
    [Export] public ArmorResources armorResources;

    public abstract void OnEquip(CombatActor itemUser);
    public abstract void OnUnequip(CombatActor itemUser);
}
