using Godot;
using System;
using Godot.Collections;

public partial class WeaponResource : ItemResource
{
    [Export] public Array<AttackProperty> WeaponProperties;
    [Export] public AttackProperty WeaponProperty;
    [Export] public Weapons WhatKindOfWeapon;
    [Export] public double WeaponDamage;
    [Export] public Dictionary<DamageTypes, int> DamageDistribution;
    [Export] public bool IsMagical;
    [Export] public WeaponType WeaponType;
    [Export] public int attackRange;
    [Export] public Array<WeaponBaseAction> Actions;

    public override Dictionary<DescriptionPanel, BaseDescription> GetDescription()
    {
        var description = new Dictionary<DescriptionPanel, BaseDescription>();
        description[DescriptionPanel.Name] = new(ItemName);
        description[DescriptionPanel.MainSprite] = new(Texture);
        description[DescriptionPanel.TypeAndRarity] = new($"{ItemRarity} {WhatKindOfWeapon}");
        description[DescriptionPanel.Damage] = new($"{WeaponDamage} Damage");
        description[DescriptionPanel.DamageDistribution] = new(DamageDistribution);
        description[DescriptionPanel.ItemEffect] = new(EffectDescription1);
        description[DescriptionPanel.ItemEffectSprite] = new(EffectTexture1);
        description[DescriptionPanel.ItemEffect2] = new(EffectDescription2);
        description[DescriptionPanel.ItemEffectSprite2] = new(EffectTexture2);
        description[DescriptionPanel.GainableSkillDescription] = new(GainSkillNameAndType);
        description[DescriptionPanel.GainableSkillSprite] = new(GainSkillSprite);
        description[DescriptionPanel.FlavourText] = new(ItemFlavourDescription);
        description[DescriptionPanel.ItemTypeText] = new(WhatKindOfWeapon.ToString());
        description[DescriptionPanel.ItemTypeSprite] = new(ItemTypeTexture1);
        description[DescriptionPanel.ItemType2Text] = new(WeaponProperties[0].ToString());
        description[DescriptionPanel.ItemType2Sprite] = new(ItemPropertyTexture1);
        if (WeaponProperties.Count > 1)
        {
            description[DescriptionPanel.ItemType3Text] = new(WeaponProperties[1].ToString());
            description[DescriptionPanel.ItemType3Sprite] = new(ItemPropertyTexture2);
        }
        description[DescriptionPanel.ItemPriceSprite] = new(ItemPriceTexture);
        description[DescriptionPanel.ItemPriceText] = new(ItemPriceText.ToString());

        return description;
    }
}