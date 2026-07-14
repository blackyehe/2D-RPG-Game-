using Godot;
using Godot.Collections;
using System;

public partial class ArmorResources : EquipableItemResource
{
    [Export] public ArmorTypes WhatKindOfArmor;
    [Export] public float ArmorDefense;
    
    public override Dictionary<DescriptionPanel, BaseDescription> GetDescription()
    {
        var description = new Dictionary<DescriptionPanel, BaseDescription>();
        description[DescriptionPanel.Name] = new(ItemName);
        description[DescriptionPanel.MainSprite] = new(Texture);
        description[DescriptionPanel.TypeAndRarity] = new($"{ItemRarity} {WhatKindOfArmor} {ItemType}");
        description[DescriptionPanel.Damage] = new($"{ArmorDefense} Defense");
        description[DescriptionPanel.ItemEffect] = new(PassiveFeature1?.FeatureDescription);
        description[DescriptionPanel.ItemEffectSprite] = new(PassiveFeature1?.PassiveFeatureSprite);
        description[DescriptionPanel.ItemEffect2] = new(PassiveFeature2?.FeatureDescription);
        description[DescriptionPanel.ItemEffectSprite2] = new(PassiveFeature2?.PassiveFeatureSprite);
        if (GainSkill != null)
        {
            description[DescriptionPanel.GainableSkillDescription] =
                new($"{GainSkill.Name}\nLevel {GainSkill.SkillLevel} {GainSkill.SkillDamageType} {GainSkill.ActionType}");
        }
        description[DescriptionPanel.GainableSkillSprite] = new(GainSkill?.Sprite);
        description[DescriptionPanel.FlavourText] = new(ItemFlavourDescription);
        description[DescriptionPanel.ItemPriceText] = new(ItemPriceText.ToString());

        return description;
    }
}