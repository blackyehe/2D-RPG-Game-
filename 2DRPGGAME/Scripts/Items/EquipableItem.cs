using Godot;
using System;

public abstract partial class EquipableItem : Item
{
    public EquipSlot CurrentlyEquippedTo;
    
    public abstract void OnEquip(CombatActor itemUser);
    public abstract void OnUnequip(CombatActor itemUser);
}
