using Godot;
using System;

public partial class BaseArmor : EquipableItem
{
    public ArmorResources ArmourResource;

    public override void _Ready()
    {
        ItemResource = (EquipableItemResource)ItemResource;
        ArmourResource = (ArmorResources)ItemResource;
    }

    public override void OnEquip(CombatActor itemUser)
    {
        itemUser.Stats.MaxDefense += ArmourResource.ArmorDefense;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(itemUser as Player);

        if (ArmourResource.GainSkill != null)
            itemUser.runtimeAbilities.Add(ArmourResource.GainSkill);
        
        ArmourResource.PassiveFeature1?.GetPassiveFeature(itemUser);
        ArmourResource.PassiveFeature2?.GetPassiveFeature(itemUser);
        GD.Print("gained:  ", ArmourResource.PassiveFeature1?.FeatureName);
        GD.Print("gained:  ", ArmourResource.PassiveFeature2?.FeatureName);
        
        itemUser.EquippedItems[ArmourResource.equipSlot] = this;
        GlobalEvents.Instance.EmitOnSkillBarChanged(itemUser.runtimeAbilities, itemUser as Player);
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(itemUser as Player);
    }

    public override void OnUnequip(CombatActor itemUser)
    {
        itemUser.Stats.MaxDefense -= ArmourResource.ArmorDefense;
        
        itemUser.runtimeAbilities.Remove(ArmourResource.GainSkill);
        
        ArmourResource.PassiveFeature1?.RemovePassiveFeature(itemUser);
        ArmourResource.PassiveFeature2?.RemovePassiveFeature(itemUser);
        
        itemUser.EquippedItems[ArmourResource.equipSlot] = null;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(itemUser as Player);
        GlobalEvents.Instance.EmitOnSkillBarChanged(itemUser.runtimeAbilities, itemUser as Player);
    }
}