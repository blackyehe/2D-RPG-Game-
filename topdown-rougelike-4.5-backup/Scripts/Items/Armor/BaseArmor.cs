using Godot;
using System;

public partial class BaseArmor : EquipableItem
{
    public ArmorResources armorResource;

    public override void _Ready()
    {
        armorResource = (ArmorResources)ItemResource;
    }

    public override void OnEquip(CombatActor itemUser)
    {
        itemUser.Stats.MaxDefense += armorResource.ArmorDefense;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(itemUser as Player);

        if (armorResource.GainSkill != null)
            itemUser.runtimeAbilities.Add(armorResource.GainSkill);
        itemUser.EquippedItems[ItemResource.equipSlot] = this;
        GlobalEvents.Instance.EmitOnSkillBarChanged(itemUser.runtimeAbilities, itemUser as Player);
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(itemUser as Player);
    }

    public override void OnUnequip(CombatActor itemUser)
    {
        itemUser.Stats.MaxDefense -= armorResource.ArmorDefense;
        
        itemUser.runtimeAbilities.Remove(armorResource.GainSkill);
        itemUser.EquippedItems[ItemResource.equipSlot] = null;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(itemUser as Player);
        GlobalEvents.Instance.EmitOnSkillBarChanged(itemUser.runtimeAbilities, itemUser as Player);
    }
}