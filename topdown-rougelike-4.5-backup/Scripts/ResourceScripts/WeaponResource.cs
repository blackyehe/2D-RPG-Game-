using Godot;
using System;
using Godot.Collections;

public partial class WeaponResource : ItemResource
{
    [Export] public Array<AttackProperty> WeaponProperties;
    [Export] public AttackProperty WhatWeapon;
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
        description[DescriptionPanel.TypeAndRarity] = new($"{ItemRarity} {WhatWeapon.Text}");
        description[DescriptionPanel.Damage] = new($"{WeaponDamage} Damage");
        description[DescriptionPanel.DamageDistribution] = new(DamageDistribution);
        description[DescriptionPanel.ItemEffect] = new(EffectDescription1);
        description[DescriptionPanel.ItemEffectSprite] = new(EffectTexture1);
        description[DescriptionPanel.ItemEffect2] = new(EffectDescription2);
        description[DescriptionPanel.ItemEffectSprite2] = new(EffectTexture2);
        if (GainSkill != null && GainSkill.IsWeaponAction)
        {
            description[DescriptionPanel.GainableSkillDescription] = new($"{GainSkill.Name}\nWeapon Action");
            description[DescriptionPanel.GainableSkillSprite] = new(GainSkill.Sprite);
            
        }
        else if (GainSkill != null && GainSkill.IsWeaponAction == false)
        {
            description[DescriptionPanel.GainableSkillDescription] = 
                new($"{GainSkill.Name}\nLevel {GainSkill.SkillLevel} {GainSkill.SkillDamageType} {GainSkill.ActionType}");
            description[DescriptionPanel.GainableSkillSprite] = new(GainSkill.Sprite);
            
        }
        description[DescriptionPanel.FlavourText] = new(ItemFlavourDescription);
        description[DescriptionPanel.ItemTypeText] = new(WhatWeapon.Text.ToString());
        description[DescriptionPanel.ItemTypeSprite] = new(WhatWeapon.Sprite);
        if (WeaponProperties.Count > 1)
        {
            description[DescriptionPanel.ItemType2Text] = new(WeaponProperties[0].Text.ToString());
            description[DescriptionPanel.ItemType2Sprite] = new(WeaponProperties[0].Sprite);
            description[DescriptionPanel.ItemType3Text] = new(WeaponProperties[1].Text.ToString());
            description[DescriptionPanel.ItemType3Sprite] = new(WeaponProperties[1].Sprite);
        }
        description[DescriptionPanel.ItemPriceText] = new(ItemPriceText.ToString());

        return description;
    }
}