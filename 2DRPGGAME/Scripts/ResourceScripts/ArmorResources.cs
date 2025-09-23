using Godot;
using Godot.Collections;
using System;

public partial class ArmorResources : ItemResource
{
    [Export] public ArmorTypes WhatKindOfArmor;
    [Export] public double ArmorDefense;

    public override Dictionary<DescriptionPanel, BaseDescription> GetDescription()
    {
        var description = new Dictionary<DescriptionPanel, BaseDescription>();
        description[DescriptionPanel.Name] = new(ItemName);
        description[DescriptionPanel.MainSprite] = new(Texture);
        description[DescriptionPanel.TypeAndRarity] = new($"{ItemRarity} {WhatKindOfArmor} {ItemType}");
        description[DescriptionPanel.Damage] = new($"{ArmorDefense} Defense");
        description[DescriptionPanel.ItemEffect] = new(EffectDescription1);
        description[DescriptionPanel.ItemEffectSprite] = new(EffectTexture1);
        description[DescriptionPanel.ItemEffect2] = new(EffectDescription2);
        description[DescriptionPanel.ItemEffectSprite2] = new(EffectTexture2);
        description[DescriptionPanel.GainableSkillDescription] = 
            new($"Level {GainSkill.SkillLevel} {GainSkill.SkillDamageType} {GainSkill.ActionType}");
        description[DescriptionPanel.GainableSkillSprite] = new(GainSkill.Sprite);
        description[DescriptionPanel.FlavourText] = new(ItemFlavourDescription);
        description[DescriptionPanel.ItemPriceText] = new(ItemPriceText.ToString());

        return description;
    }
}